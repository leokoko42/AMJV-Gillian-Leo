using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class EntityBehavior : MonoBehaviour
{
    private BasicEntity allyEntity;
    private Rigidbody allyRigidBody;
    private bool stunned, isOnCooldown, touchingGround;
    private LayerMask layerMaskWalls, layerMaskGround;
    private NavMeshAgent allyNavMeshAgent;
    public UnityEvent<float> attacked_enemy;
    private GameManager game_manager;
    private Entity.Behavior entityBehavior;
    private List<GameObject> allies_list, oponents_list;
    private int currentNavMeshMask;

    void Start()
    {
        allyEntity = GetComponent<BasicEntity>();
        entityBehavior = allyEntity.GetBehavior();

        allyRigidBody = GetComponent<Rigidbody>();

        layerMaskWalls = LayerMask.GetMask("Walls");
        layerMaskGround = LayerMask.GetMask("Ground");
        isOnCooldown = false;

        allyNavMeshAgent = GetComponent<NavMeshAgent>();
        allyNavMeshAgent.speed = allyEntity.GetSpeed();
        allyNavMeshAgent.stoppingDistance = allyEntity.GetRange();

        touchingGround = true;
        stunned = false;

        game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        UpdateGroundArea();
        RefreshEntitiesLists();
    }
    
    public void RefreshEntitiesLists() {
        if (allyEntity.GetIsEnemy()) {
            allies_list = game_manager.enemy_list;
            oponents_list = game_manager.ally_list;
        }
        else {
            allies_list = game_manager.ally_list;
            oponents_list = game_manager.enemy_list;
        }
    }

    void FixedUpdate() {
        RefreshEntitiesLists();
        UpdateEntityMovement();
    }

    private void UpdateIsOnGround() {
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, 0.6f, layerMaskGround);
    }

    private void UpdateGroundArea() {
        NavMeshHit hit;
        allyNavMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0, out hit);
        if (!allyEntity.GetIsEnemy()) {
            Debug.Log(currentNavMeshMask);
        }
        if (hit.mask != currentNavMeshMask) {
            currentNavMeshMask = hit.mask;
            if (hit.mask == 1) {
                allyNavMeshAgent.speed = 3;
            }
            else if (hit.mask == 8) {
                allyNavMeshAgent.speed = 1;
            }
        }
    }

    //Methods designed to find the different targets
    private (GameObject,float) getTargetEntity() {
        if (entityBehavior == Entity.Behavior.Neutral) {
            return getClosestOponent(oponents_list);
        }
        else if (entityBehavior == Entity.Behavior.Offense) {
            return getKingOf(oponents_list);
        }
        else if (entityBehavior == Entity.Behavior.Defense) {
            return getClosestOponentOfKing(oponents_list, allies_list);
        }   
        else {
            throw new Exception("Entity doesn't have a valid Behavior");
        }
    }
    private (GameObject,float) getClosestOponentOfKing(List<GameObject> oponents, List<GameObject> allies) {
        (GameObject allyKing, float kingDistance) = getKingOf(allies);

        EntityBehavior allyKingBehavior = allyKing.GetComponent<EntityBehavior>();
        (GameObject allyKingClosestOponent, float oponentDistanceFromKing) = allyKingBehavior.getClosestOponent(oponents);
        float distance = Vector3.Distance(transform.position, allyKingClosestOponent.transform.position);
        return (allyKingClosestOponent, distance);
    }
    private (GameObject,float) getClosestOponent(List<GameObject> oponents) {
        float minDistanceNotObstructed = -1f; 
        float minDistanceObstructed = -1f;
        GameObject closestOponentNotObstructed = null;
        GameObject closestOponentObstructed = null;

        foreach (GameObject oponent in oponents)
        {
            float distance = Vector3.Distance(transform.position, oponent.transform.position);

            bool isObstructed = isEntityObstructed(oponent, distance);
           
            if (isObstructed) {
                if (minDistanceObstructed == -1f || distance < minDistanceObstructed) {
                    closestOponentObstructed = oponent;
                    minDistanceObstructed = distance;
                }
            }
            else {
                if (minDistanceNotObstructed == -1f || distance < minDistanceNotObstructed) {
                    closestOponentNotObstructed = oponent;
                    minDistanceNotObstructed = distance;
                }
            }
        }
        if (closestOponentNotObstructed!=null) {
            return (closestOponentNotObstructed, minDistanceNotObstructed);
        }
        else {
            return (closestOponentObstructed, minDistanceObstructed);
        }
    }

    private bool isEntityObstructed(GameObject entity, float distance) {
        Vector3 rayDirection =  entity.transform.position-transform.position;
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.5f, 0);
        if (Physics.Raycast(rayOrigin, rayDirection, distance, layerMaskWalls)) {
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        }
        else {Debug.DrawRay(rayOrigin, rayDirection, Color.green);}

        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, distance, layerMaskWalls);
        return isObstructed;
    }

    private (GameObject,float) getKingOf(List<GameObject> entities) {
        foreach (GameObject entity in entities) {
            BasicEntity oponentEntity = entity.GetComponent<BasicEntity>();
            if (oponentEntity.GetIsKing()) {
                float distance = Vector3.Distance(entity.transform.position,transform.position);
                return (entity, distance);
            }
        }
        throw new Exception("No oponent king");
    }

    //Methods designed to dictate the actions of the Entity
    public void UpdateEntityMovement() {
        UpdateIsOnGround();
        if (stunned || !allyEntity.GetIsActive()) 
        {
            if (allyNavMeshAgent.enabled) {
                allyNavMeshAgent.enabled = false;
                allyRigidBody.linearVelocity = new Vector3();
            }
            return;
        }
        else 
        {
            allyNavMeshAgent.enabled = true;
            UpdateGroundArea();
            GameObject targetOponent;
            float distance;

            (targetOponent,distance) = getTargetEntity();
            float range = allyEntity.GetRange();
            if (distance <= range && !isEntityObstructed(targetOponent, distance)) {
                if (!isOnCooldown) {
                    AttackOther(targetOponent, range);
                }
            }
            else {
                MoveToGameObject(targetOponent);
            }
        }
    }


    //Usefull methods for basic actions
    private void MoveToGameObject(GameObject gameObject) {
        allyNavMeshAgent.SetDestination(gameObject.transform.position);
    }

    private void AttackOther(GameObject oponent, float range) {
        if (range<5) {}
        HealthManager oponentHealth = oponent.GetComponent<HealthManager>();
        int attack = allyEntity.GetAttack();
        int attackSpeed = allyEntity.GetAttackSpeed();

        oponentHealth.Damage(attack);
        attacked_enemy.Invoke(attack);

        StartCoroutine(AttackCooldown(attackSpeed));
    }

    private void UseUltimate() {
        throw new NotImplementedException();
    }

    //Differents corroutines
    IEnumerator AttackCooldown(int cooldown)
    {
        isOnCooldown = true;
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }

    public void ApplyKnockback(Vector3 origin, float stunCooldown) {
        StartCoroutine(Knockback(origin, stunCooldown));
    }

    IEnumerator Knockback(Vector3 origin, float stunCooldown) {
        StartCoroutine(StunnedForSeconds(stunCooldown));
        while (allyNavMeshAgent.enabled) {
            yield return new WaitForSeconds(0.1f);
        }
        //yield return new WaitForSeconds(0.1f);
        //GetComponent<Rigidbody>().AddExplosionForce(200, new Vector3(origin.x, origin.y-0.5f, origin.z) , 10, 10);
    }
    IEnumerator StunnedForSeconds(float cooldown) {
        stunned = true;
        yield return new WaitForSeconds(cooldown);
        while(!touchingGround) {
            yield return new WaitForSeconds(0.1f);
        }
        stunned = false;
    }
}

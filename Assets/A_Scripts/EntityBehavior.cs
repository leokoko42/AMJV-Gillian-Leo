using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class EntityBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private BasicEntity allyEntity;
    private BasicEntity oponentEntity;
    private Rigidbody allyRigidBody;
    private bool isOnCooldown;
    private LayerMask layerMaskWalls, layerMaskGround;
    [SerializeField] private bool touchingGround;
    public bool stunned;
    NavMeshAgent allyNavMeshAgent;
    public UnityEvent<float> attacked_enemy;
    private GameObject game_manager_object;
    private GameManager game_manager;
    private Entity.Behavior entityBehavior;

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

        game_manager_object = GameObject.Find("GameManager");
        game_manager = game_manager_object.GetComponent<GameManager>();
    }
    
    void Update()
    {
        
    }

    void FixedUpdate() {
        UpdateEntityMovement();
    }

    private void IsOnGound() {
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, 0.6f, layerMaskGround);
    }

    //Methods designed to find the different targets
    private (GameObject,float) getTargetEntity() {
        List<GameObject> oponents;
        if (allyEntity.GetIsEnemy()) {
            oponents = game_manager.ally_list;
        }
        else {
            oponents = game_manager.enemy_list;
        }
        
        
        if (entityBehavior == Entity.Behavior.Neutral) {
            return getClosestOponent(oponents);
        }
        else if (entityBehavior == Entity.Behavior.Offense) {
            return getKingOf(oponents);
        }
        else if (entityBehavior == Entity.Behavior.Defense) {
            return getClosestOponentOfKing(oponents);
        }   
        else {
            throw new Exception("Entity doesn't have a valid Behavior");
        }
    }
    private (GameObject,float) getClosestOponentOfKing(List<GameObject> oponents) {
        List<GameObject> allies = game_manager.ally_list;
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
            oponentEntity = entity.GetComponent<BasicEntity>();
            if (oponentEntity.GetIsKing()) {
                float distance = Vector3.Distance(entity.transform.position,transform.position);
                return (entity, distance);
            }
        }
        throw new Exception("No oponent king");
    }

    //Methods designed to dictate the actions of the Entity
    public void UpdateEntityMovement() {
        if (stunned || !allyEntity.GetIsActive()) {
            IsOnGound();
            if (allyNavMeshAgent.enabled) {
                allyNavMeshAgent.enabled = false;
                allyRigidBody.linearVelocity = new Vector3();
            }
            return;
        }
        allyNavMeshAgent.enabled = true;
        GameObject targetOponent;
        float distance;

        (targetOponent,distance) = getTargetEntity();

        if (distance <= allyEntity.GetRange() && !isEntityObstructed(targetOponent, distance)) {
            if (!isOnCooldown) {
                AttackOther(targetOponent);
            }
        }
        else {
            MoveToGameObject(targetOponent);
        }
    }


    //Usefull methods for basic actions
    private void MoveToGameObject(GameObject gameObject) {
        allyNavMeshAgent.SetDestination(gameObject.transform.position);
    }

    private void AttackOther(GameObject oponent) {
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

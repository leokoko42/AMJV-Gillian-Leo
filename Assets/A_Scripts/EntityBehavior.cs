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
    [SerializeField] private GameObject entityBullet;
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
    private float entity_speed;

    public float total_damage_dealt;
    public float total_damage_taken;
    public float total_damage_absorbed;
    public float total_enemies_defeated;
    public float total_mana_charged;
    public float total_abilities_used;
    public float total_health_recovered;

    void Start()
    {
        allyEntity = GetComponent<BasicEntity>();
        entityBehavior = allyEntity.GetBehavior();

        allyRigidBody = GetComponent<Rigidbody>();

        // A mettre en public dans le game Manager ?
        layerMaskWalls = LayerMask.GetMask("Walls");
        layerMaskGround = LayerMask.GetMask("Ground");
        // ----

        isOnCooldown = false;

        entity_speed = allyEntity.GetSpeed();

        allyNavMeshAgent = GetComponent<NavMeshAgent>();
        allyNavMeshAgent.stoppingDistance = allyEntity.GetRange();

        touchingGround = true;
        stunned = false;

        game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        
        total_damage_dealt = 0;
        total_damage_taken = 0;
        total_damage_absorbed = 0;
        total_enemies_defeated = 0;
        total_mana_charged = 0;
        total_abilities_used = 0;
        total_health_recovered = 0;

        UpdateGroundAreaType();
        RefreshEntitiesLists();
    }

    void Update() {

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
        // Update que si le manager le fait sinon useless
        RefreshEntitiesLists();
        // --

        UpdateEntityMovement();
    }

    private void UpdateIsOnGround() {
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, 0.7f, layerMaskGround);
    }

    private void UpdateGroundAreaType() {
        NavMeshHit hit;
        allyNavMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0, out hit);
        if (hit.mask != currentNavMeshMask) {
            currentNavMeshMask = hit.mask;
            if (hit.mask == 1) //On grass ground
            {
                allyNavMeshAgent.speed = entity_speed;
            }
            else if (hit.mask == 8) //On sand ground
            {
                allyNavMeshAgent.speed = entity_speed * 0.5f;
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
        // A changer avec le gameManager
        (GameObject allyKing, float kingDistance) = getKingOf(allies);
        EntityBehavior allyKingBehavior = allyKing.GetComponent<EntityBehavior>();
        // -----
        (GameObject allyKingClosestOponent, float oponentDistanceFromKing) = allyKingBehavior.getClosestOponent(oponents);
        float distance = Vector3.Distance(transform.position, allyKingClosestOponent.transform.position);
        return (allyKingClosestOponent, distance);
    }

    // A changer pour éviter un trop grand nombre de raycast ?
    private (GameObject,float) getClosestOponent(List<GameObject> oponents) {
        float minDistanceNotObstructed = float.PositiveInfinity; 
        float minDistanceObstructed = float.PositiveInfinity;
        GameObject closestOponentNotObstructed = null;
        GameObject closestOponentObstructed = null;

        foreach (GameObject oponent in oponents)
        {
            float distance = Vector3.Distance(transform.position, oponent.transform.position);

            bool isObstructed = isEntityObstructed(oponent, distance);
           
            if (isObstructed) {
                if (distance < minDistanceObstructed) {
                    closestOponentObstructed = oponent;
                    minDistanceObstructed = distance;
                }
            }
            else {
                if (distance < minDistanceNotObstructed) {
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
        // Mettre un transform pour la position des yeux sur le long terme 
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.5f, 0);

        /*
        if (Physics.Raycast(rayOrigin, rayDirection, distance, layerMaskWalls)) {
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        }
        else {Debug.DrawRay(rayOrigin, rayDirection, Color.green);}*/

        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, distance, layerMaskWalls);
        return isObstructed;
    }

    // A déplacer dans GameManager
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
        if (stunned || !allyEntity.GetIsActive()) //Enable the rigidbody & physics
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
            UpdateGroundAreaType();
            GameObject targetOponent;
            float distance;

            (targetOponent,distance) = getTargetEntity();
            float range = allyEntity.GetRange();
            if (distance <= range && !isEntityObstructed(targetOponent, distance)) {
                if (!isOnCooldown) {
                    MoveToGameObject(gameObject); //Maybe use a better alternative to stop the movement ?
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
        float attack = allyEntity.GetAttack();
        float attackSpeed = allyEntity.GetAttackSpeed();

        if (range>5) { //Add a bool variable to ensure if an entity is ranged or not ?
            RangedAttack(oponent, attack);
        }
        else {
            MeleeAttack(oponent, attack);
        }
        StartCoroutine(AttackCooldown(attackSpeed));
    }

    private void RangedAttack(GameObject oponent, float attack) {
        GameObject bullet = Instantiate(entityBullet, transform.position + Vector3.forward*0.5f, transform.rotation);
        BulletMovment bulletMovment = bullet.GetComponent<BulletMovment>(); 
        bulletMovment.Init(attack, oponent, gameObject);
    }

    private void MeleeAttack(GameObject oponent, float attack) {
        HealthManager oponentHealth = oponent.GetComponent<HealthManager>();

        float dmg_dealt = oponentHealth.Damage(attack);
        ChangeDamageDealt(dmg_dealt);
        attacked_enemy.Invoke(dmg_dealt);
    }

    private void UseUltimate() {
        ChangeAbilitiesUsed();
        throw new NotImplementedException();
    }

    //Differents corroutines
    IEnumerator AttackCooldown(float cooldown)
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
        /*while (allyNavMeshAgent.enabled) {
            yield return new WaitForSeconds(0.1f);
        }*/
        yield  return new WaitForSeconds(0f);
        GetComponent<Rigidbody>().AddExplosionForce(200, new Vector3(origin.x, origin.y-0.5f, origin.z) , 10, 10);
    }
    IEnumerator StunnedForSeconds(float cooldown) {
        stunned = true;
        yield return new WaitForSeconds(cooldown);
        while(!touchingGround) {
            yield return new WaitForSeconds(0.1f);
        }
        stunned = false;
    }

    public void ChangeDamageDealt(float dmg)
    {
        total_damage_dealt += dmg;
    }
    public void ChangeDamageTaken(float dmg)
    {
        total_damage_taken += dmg;
    }
    public void ChangeDamageAbsorbed(float dmg)
    {
        total_damage_absorbed += dmg;
    }
    public void ChangeEnemiesDefeated()
    {
        total_enemies_defeated++;
    }
    public void ChangeManaCharged(float mana)
    {
        total_mana_charged += mana;
    }
    public void ChangeAbilitiesUsed()
    {
        total_abilities_used++;
    }
    public void ChangeHealthRecovered(float heal)
    {
        total_health_recovered += heal;
    }

}

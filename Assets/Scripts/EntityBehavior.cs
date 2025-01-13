using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class EntityBehavior : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private BasicEntity allyEntity;
    private BasicEntity enemyEntity;
    private bool isOnCooldown;
    private LayerMask layerMask;
    [SerializeField] private bool touchingGround;
    public bool stunned;
    NavMeshAgent allyNavMeshAgent;
    public UnityEvent<float> attacked_enemy;
    void Start()
    {
        allyEntity = GetComponent<BasicEntity>();
        layerMask = LayerMask.GetMask("Walls");
        isOnCooldown = false;

        allyNavMeshAgent = GetComponent<NavMeshAgent>();
        allyNavMeshAgent.speed = allyEntity.GetSpeed();
        allyNavMeshAgent.stoppingDistance = allyEntity.GetRange();

        touchingGround = true;
        stunned = false;
    }
    
    void Update()
    {
        
    }

    void FixedUpdate() {
        UpdateEntityMovement();
    }

    private void IsOnGound() {
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, 0.6f);
    }

    private bool EntityCanMove() {
        if (touchingGround) {
            return true;
        }
        else {
            return false;
        }
    }

    //Methods designed to find the different targets
    private (GameObject,float) getTargetEntity() {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemies");
        Entity.Behavior entityBehavior = allyEntity.GetBehavior();
        if (entityBehavior == Entity.Behavior.Neutral) {
            return getClosestEnemy(enemies);
        }
        else if (entityBehavior == Entity.Behavior.Offense) {
            return getKingOf(enemies);
        }
        else if (entityBehavior == Entity.Behavior.Defense) {
            return getClosestEnemyOfKing(enemies);
        }   
        else {
            throw new Exception("Entity doesn't have a valid Behavior");
        }
    }
    private (GameObject,float) getClosestEnemyOfKing(GameObject[] enemies) {
        GameObject[] allies = GameObject.FindGameObjectsWithTag("Allies");
        (GameObject allyKing, float kingDistance) = getKingOf(allies);

        EntityBehavior allyKingBehavior = allyKing.GetComponent<EntityBehavior>();
        (GameObject allyKingClosestEnemy, float enemyDistanceFromKing) = allyKingBehavior.getClosestEnemy(enemies);
        float distance = Vector3.Distance(transform.position, allyKingClosestEnemy.transform.position);
        return (allyKingClosestEnemy, distance);
    }
    private (GameObject,float) getClosestEnemy(GameObject[] enemies) {
        float minDistanceNotObstructed = -1f; 
        float minDistanceObstructed = -1f;
        GameObject closestEnemyNotObstructed = null;
        GameObject closestEnemyObstructed = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            bool isObstructed = isEntityObstructed(enemy, distance);
           
            if (isObstructed) {
                if (minDistanceObstructed == -1f || distance < minDistanceObstructed) {
                    closestEnemyObstructed = enemy;
                    minDistanceObstructed = distance;
                }
            }
            else {
                if (minDistanceNotObstructed == -1f || distance < minDistanceNotObstructed) {
                    closestEnemyNotObstructed = enemy;
                    minDistanceNotObstructed = distance;
                }
            }
        }
        if (closestEnemyNotObstructed!=null) {
            return (closestEnemyNotObstructed, minDistanceNotObstructed);
        }
        else {
            return (closestEnemyObstructed, minDistanceObstructed);
        }
    }

    private bool isEntityObstructed(GameObject entity, float distance) {
        Vector3 rayDirection =  entity.transform.position-transform.position;
        Vector3 rayOrigin = transform.position + new Vector3(0, 0.5f, 0);
        if (Physics.Raycast(rayOrigin, rayDirection, distance, layerMask)) {
            Debug.DrawRay(rayOrigin, rayDirection, Color.red);
        }
        else {Debug.DrawRay(rayOrigin, rayDirection, Color.green);}

        bool isObstructed = Physics.Raycast(rayOrigin, rayDirection, distance, layerMask);
        return isObstructed;
    }

    private (GameObject,float) getKingOf(GameObject[] entities) {
        foreach (GameObject entity in entities) {
            enemyEntity = entity.GetComponent<BasicEntity>();
            if (enemyEntity.GetIsKing()) {
                float distance = Vector3.Distance(entity.transform.position,transform.position);
                return (entity, distance);
            }
        }
        throw new Exception("No enemy king");
    }

    //Methods designed to dictate the actions of the Entity
    public void UpdateEntityMovement() {
        if (stunned || !allyEntity.GetIsActive()) {
            allyNavMeshAgent.enabled = false;
            return;
        }
        allyNavMeshAgent.enabled = true;
        GameObject targetEnemy;
        float distance;

        (targetEnemy,distance) = getTargetEntity();
        enemyEntity = targetEnemy.GetComponent<BasicEntity>();

        if (distance <= allyEntity.GetRange() && !isEntityObstructed(targetEnemy, distance)) {
            if (!isOnCooldown) {
                AttackEnemy(targetEnemy);
            }
        }
        else {
            MoveToEnemy(targetEnemy);
        }
    }


    //Usefull methods for basic actions
    private void MoveToEnemy(GameObject enemy) {
        allyNavMeshAgent.SetDestination(enemy.transform.position);
    }

    private void AttackEnemy(GameObject enemy) {
        HealthManager enemyHealth = enemy.GetComponent<HealthManager>();
        int attack = allyEntity.GetAttack();
        int attackSpeed = allyEntity.GetAttackSpeed();

        enemyHealth.Damage(attack);
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
        GetComponent<Rigidbody>().AddExplosionForce(200, origin, 10, 1);
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

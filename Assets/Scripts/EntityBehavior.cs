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
    [SerializeField] NavMeshAgent ally;
    public GameObject[] enemies;
    private BasicEntity allyEntity;
    private BasicEntity enemyEntity;
    private bool isOnCooldown;
    private LayerMask layerMask;
    public UnityEvent<float> attacked_enemy;
    void Start()
    {
        allyEntity = GetComponent<BasicEntity>();
        layerMask = LayerMask.GetMask("Walls");
        isOnCooldown = false;
    }
    
    private (GameObject,float) getClosestEnemy() {
        float minDistanceNotObstructed = -1f; 
        float minDistanceObstructed = -1f;
        GameObject closestEnemyNotObstructed = null;
        GameObject closestEnemyObstructed = null;
        enemies = GameObject.FindGameObjectsWithTag("Enemies");

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);

            Vector3 rayDirection =  enemy.transform.position-transform.position;

            if (Physics.Raycast(transform.position, rayDirection, Mathf.Infinity, layerMask)) {
                Debug.DrawRay(transform.position, rayDirection, Color.red);
            }
            else {Debug.DrawRay(transform.position, rayDirection, Color.green);}

            bool isObstructed = Physics.Raycast(transform.position, rayDirection, Mathf.Infinity, layerMask);
           
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

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate() {
        GameObject targetEnemy;
        float distance;

        (targetEnemy,distance) = getClosestEnemy();
        Debug.Log(targetEnemy);
        enemyEntity = targetEnemy.GetComponent<BasicEntity>();

        if (distance <= 3) {
            if (!isOnCooldown) {
                AttackEnemy(targetEnemy);
            }
        }
        else {
            MoveToEnemy(targetEnemy);
        }
    }

    private void MoveToEnemy(GameObject enemy) {
        
        ally.SetDestination(enemy.transform.position);
    }

    private void AttackEnemy(GameObject enemy) {
        HealthManager enemyHealth = enemy.GetComponent<HealthManager>();
        int attack = allyEntity.GetAttack();
        int attackSpeed = allyEntity.GetAttackSpeed();

        enemyHealth.Damage(attack);
        attacked_enemy.Invoke(attack);
        Debug.Log("ATTACK !");

        isOnCooldown = true;
        StartCoroutine(AttackCooldown(attackSpeed));
    }

    IEnumerator AttackCooldown(int cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        isOnCooldown = false;
    }
}

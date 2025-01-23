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
    private UnitPlacement unit_placement;
    private Entity.Behavior entityBehavior;
    private List<GameObject> allies_list, oponents_list;
    private int currentNavMeshMask;

    [SerializeField] private GameObject crown;
    [SerializeField] private GameObject health_bar;
    [SerializeField] private GameObject mana_bar;
    [SerializeField] private GameObject behavior_menu;
    public UnityEvent show_menu;
    public UnityEvent hide_menu;

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

        layerMaskWalls = LayerMask.GetMask("Walls");
        layerMaskGround = LayerMask.GetMask("Ground");
        isOnCooldown = false;

        allyNavMeshAgent = GetComponent<NavMeshAgent>();
        allyNavMeshAgent.speed = allyEntity.GetSpeed();
        allyNavMeshAgent.stoppingDistance = allyEntity.GetRange();

        touchingGround = true;
        stunned = false;

        game_manager = GameObject.Find("GameManager").GetComponent<GameManager>();
        unit_placement = GameObject.Find("PlacementManager").GetComponent<UnitPlacement>();

        //crown = GameObject.Find("Crown");
        crown.SetActive(false);
        //health_bar = GameObject.Find("HealthBar");
        //mana_bar = GameObject.Find("ManaBar");
        //behavior_menu = GameObject.Find("BehaviorMenu");

        total_damage_dealt = 0;
        total_damage_taken = 0;
        total_damage_absorbed = 0;
        total_enemies_defeated = 0;
        total_mana_charged = 0;
        total_abilities_used = 0;
        total_health_recovered = 0;
        UpdateGroundArea();
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
        RefreshEntitiesLists();
        UpdateEntityMovement();
    }

    private void UpdateIsOnGround() {
        touchingGround = Physics.Raycast(transform.position, -Vector3.up, 0.6f, layerMaskGround);
    }

    private void UpdateGroundArea() {
        NavMeshHit hit;
        allyNavMeshAgent.SamplePathPosition(NavMesh.AllAreas, 0, out hit);
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
                    MoveToGameObject(gameObject);
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
        isOnCooldown = true;
        float attack = allyEntity.GetAttack();
        float attackSpeed = allyEntity.GetAttackSpeed();

        if (range>5) {
            GameObject bullet = Instantiate(entityBullet, transform.position + Vector3.forward*0.5f, transform.rotation);
            BulletMovment bulletMovment = bullet.GetComponent<BulletMovment>(); 
            bulletMovment.Init(attack, oponent, gameObject);
        }
        else {
            HealthManager oponentHealth = oponent.GetComponent<HealthManager>();

            float dmg_dealt = oponentHealth.Damage(attack);
            ChangeDamageDealt(dmg_dealt);
            attacked_enemy.Invoke(dmg_dealt);
        }
        StartCoroutine(AttackCooldown(attackSpeed));
    }

    private void UseUltimate() {
        ChangeAbilitiesUsed();
        throw new NotImplementedException();
    }

    //Differents corroutines
    IEnumerator AttackCooldown(float cooldown)
    {
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

    public void AddCrown()
    {
        crown.SetActive(true);
        Vector3 hb_pos = health_bar.transform.position;
        health_bar.transform.position = new Vector3(hb_pos.x, hb_pos.y+25, hb_pos.z);
        Vector3 mb_pos = mana_bar.transform.position;
        mana_bar.transform.position = new Vector3(mb_pos.x, mb_pos.y + 25, mb_pos.z);
        Vector3 bm_pos = behavior_menu.transform.position;
        behavior_menu.transform.position = new Vector3(mb_pos.x, mb_pos.y + 25, mb_pos.z);
    }

    public void RemoveCrown()
    {
        crown.SetActive(false);
        Vector3 hb_pos = health_bar.transform.position;
        health_bar.transform.position = new Vector3(hb_pos.x, hb_pos.y - 25, hb_pos.z);
        Vector3 mb_pos = mana_bar.transform.position;
        mana_bar.transform.position = new Vector3(mb_pos.x, mb_pos.y - 25, mb_pos.z);
        Vector3 bm_pos = behavior_menu.transform.position;
        behavior_menu.transform.position = new Vector3(mb_pos.x, mb_pos.y - 25, mb_pos.z);
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

    public void ShowMenu()
    {
        show_menu.Invoke();
    }

    public void HideMenu()
    {
        hide_menu.Invoke();
    }

    public void OffenseBehavior()
    {
        allyEntity.SetBehavior(Entity.Behavior.Offense);
    }

    public void NeutralBehavior()
    {
        allyEntity.SetBehavior(Entity.Behavior.Neutral);
    }

    public void DefenseBehavior()
    {
        allyEntity.SetBehavior(Entity.Behavior.Defense);
    }

    public void SetKing()
    {
        game_manager.SetAllyKing(gameObject);
    }

    public void Sell()
    {
        unit_placement.SellEntity(gameObject);
    }

}

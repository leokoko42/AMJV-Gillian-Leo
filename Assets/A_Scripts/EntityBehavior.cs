using System;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Analytics;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

public class EntityBehavior : MonoBehaviour
{
    public enum Ability {Warrior, Archer, Tank, Healer, Monk, Summoner, Shaman,Hammer}
    [SerializeField] private Ability ability;
    [SerializeField] private GameObject entityBullet;
    [SerializeField] private BasicEntity allyEntity;
    [SerializeField] private Rigidbody allyRigidBody;
    [SerializeField] private AbilityManager allyAbility;
    [SerializeField] private NavMeshAgent allyNavMeshAgent;
    public bool stunned, isOnCooldown, touchingGround;
    private LayerMask layerMaskWalls, layerMaskGround;
    public UnityEvent<float> attacked_enemy;
    private GameManager game_manager;
    private UnitPlacement unit_placement;
    private Entity.Behavior entityBehavior;
    private List<GameObject> allies_list, oponents_list;
    private int currentNavMeshMask;
    private float entity_speed;
    public float total_damage_dealt,total_damage_taken,total_damage_absorbed,total_enemies_defeated;
    public float total_mana_charged,total_abilities_used,total_health_recovered;
    [SerializeField] private GameObject crown;
    [SerializeField] private GameObject health_bar;
    [SerializeField] private GameObject mana_bar;
    [SerializeField] private GameObject behavior_menu;
    public UnityEvent show_menu, hide_menu;
    [SerializeField] private GameObject left_eye, right_eye;
    [SerializeField] private Material offense_mat, neutral_mat, defense_mat;

    [SerializeField] private GameObject heal_halo;
    [SerializeField] private GameObject power_belt;
    [SerializeField] private GameObject poison_vial;
    [SerializeField] private GameObject revenge_mask;
    [SerializeField] private GameObject resurrection_earring;

    private string item_equipped;
    private GameObject item_visual;

    private int allies_on_start;
    private float revenge_mult;
    [SerializeField] ParticleSystem fireParticles, iceParticles, poisonParticles;
    private float initialDef;

    void Start()
    {
        entityBehavior = allyEntity.GetBehavior();

        // A mettre en public dans le game Manager ?
        layerMaskWalls = LayerMask.GetMask("Walls");
        layerMaskGround = LayerMask.GetMask("Ground");
        // ----

        isOnCooldown = false;

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

        item_equipped = "";

        total_damage_dealt = 0;
        total_damage_taken = 0;
        total_damage_absorbed = 0;
        total_enemies_defeated = 0;
        total_mana_charged = 0;
        total_abilities_used = 0;
        total_health_recovered = 0;


        UpdateGroundAreaType();
        RefreshEntitiesLists();

        revenge_mult = 1f;
        initialDef = allyEntity.GetDef();
        fireParticles.Stop();
        iceParticles.Stop();
        poisonParticles.Stop();
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
            entity_speed = allyEntity.GetSpeed();
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
            GameObject targetOponent;
            float distance;

            (targetOponent,distance) = getTargetEntity();
            float range = allyEntity.GetRange();

            bool targetInRange = (distance <= range && !isEntityObstructed(targetOponent, distance));
            bool abilityAvailable = (allyEntity.GetMana() == allyEntity.GetMaxMana());

            if (!isOnCooldown && allyAbility.ActivateAbility(abilityAvailable, ability, gameObject, targetOponent, targetInRange)) {
                if (item_equipped == "HealHalo") 
                    gameObject.GetComponent<HealthManager>().Heal(allyEntity.GetMaxHP() * DataHolder.GetHaloHealPercentage());
                allyEntity.SetMana(0);
                StartCoroutine(AttackCooldown());
            }
            else if (!isOnCooldown && targetInRange) {
                allyNavMeshAgent.enabled = false;
                AttackOther(targetOponent, range);
            }
            else {
                allyNavMeshAgent.enabled = true;
                UpdateGroundAreaType();
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

        if (range>5) { //Add a bool variable to ensure if an entity is ranged or not ?
            RangedAttack(oponent, attack);
        }
        else {
            MeleeAttack(oponent, attack);
        }
        StartCoroutine(AttackCooldown());
    }

    private void RangedAttack(GameObject oponent, float attack) {
        GameObject bullet = Instantiate(entityBullet, transform.position + Vector3.forward*0.5f, transform.rotation);
        BulletMovment bulletMovment = bullet.GetComponent<BulletMovment>(); 
        bulletMovment.Init(attack, oponent, gameObject, item_equipped == "PoisonVial" ? attack * DataHolder.vial_poison_precentage : 0);
    }

    private void MeleeAttack(GameObject oponent, float attack) {
        HealthManager oponentHealth = oponent.GetComponent<HealthManager>();

        float dmg_dealt = oponentHealth.Damage(attack);
        if (item_equipped == "PoisonVial")
        {
            EntityBehavior eb = oponent.GetComponent<EntityBehavior>();
            eb.ApplyPoison(attack * DataHolder.vial_poison_precentage);
        }
        ChangeDamageDealt(dmg_dealt);
        attacked_enemy.Invoke(dmg_dealt);
    }

    //Differents corroutines
    public IEnumerator AttackCooldown()
    {
        float attackSpeed = allyEntity.GetAttackSpeed();
        isOnCooldown = true;
        yield return new WaitForSeconds(attackSpeed);
        isOnCooldown = false;
    }

    //Special Status

        //Knockback
    public void ApplyKnockback(Vector3 origin, float stunCooldown, float strenght) {
        StartCoroutine(Knockback(origin, stunCooldown, strenght));
    }

    IEnumerator Knockback(Vector3 origin, float stunCooldown, float strenght) {
        StartCoroutine(StunnedForSeconds(stunCooldown));
        yield  return new WaitForSeconds(0f);
        GetComponent<Rigidbody>().AddExplosionForce(40 * strenght, origin , 10, strenght/5);
    }
    IEnumerator StunnedForSeconds(float cooldown) {
        stunned = true;
        yield return new WaitForSeconds(cooldown);
        while(!touchingGround) {
            yield return new WaitForSeconds(0.1f);
        }
        stunned = false;
    }

        //Fire
    private bool isOnFire = false;
    private void Update() {
        if (Input.GetKeyDown("space")) {
            Debug.Log("FIRE");
            ApplyPoison(10);
        }
    }
    public void ApplyFire(float time) {
        if (isOnFire) {
            StopCoroutine(FireForSeconds(time));
            StartCoroutine(FireForSeconds(time));
        }
        else {
            StartCoroutine(FireForSeconds(time));
            StartCoroutine(FireDamage());
        }
    }
    private IEnumerator FireForSeconds(float cooldown) {
        isOnFire = true;
        fireParticles.Play();
        yield return new WaitForSeconds(cooldown);
        isOnFire = false;
        fireParticles.Stop();
    }
    private IEnumerator FireDamage() {
        HealthManager allyHealth = GetComponent<HealthManager>();
        while(isOnFire) {
            allyHealth.TrueDamage(allyEntity.GetMaxHP()/100);
            Debug.Log(allyEntity.GetMaxHP()/100);
            yield return new WaitForSeconds(1);
        }
    }

        //Poison
    private int nbPoison = 0;
    public void ApplyPoison(float amount) {
        StartCoroutine(PoisonForSeconds(amount));
    }
    private IEnumerator PoisonForSeconds(float amount) {
        nbPoison++;
        float totalDebuff = Mathf.Pow(0.95f, amount);
        allyEntity.AddDefMultiplier(totalDebuff);
        poisonParticles.Play();
        yield return new WaitForSeconds(10);
        nbPoison--;
        allyEntity.RemoveDefMultiplier(totalDebuff);
        if (nbPoison == 0) {
            poisonParticles.Stop();
        }
    }
        //Ice
    private int nbIce = 0;
    public void ApplyIce(float time) {
        StartCoroutine(IceForSeconds(time));
    }
    private IEnumerator IceForSeconds(float time) {
        nbIce++;
        allyEntity.AddCooldownMultiplier(1.1f);
        iceParticles.Play();
        yield return new WaitForSeconds(time);
        nbIce--;
        allyEntity.RemoveCooldownMultiplier(1.1f);
        if (nbIce == 0) {
            iceParticles.Stop();
        }
    }

    public void AddCrown()
    {
        crown.SetActive(true);
        Vector3 hb_pos = health_bar.transform.position;
        health_bar.transform.position = new Vector3(hb_pos.x, hb_pos.y + 0.25f, hb_pos.z);
        Vector3 mb_pos = mana_bar.transform.position;
        mana_bar.transform.position = new Vector3(mb_pos.x, mb_pos.y + 0.25f, mb_pos.z);
        Vector3 bm_pos = behavior_menu.transform.position;
        behavior_menu.transform.position = new Vector3(mb_pos.x, mb_pos.y + 0.25f, mb_pos.z);
    }

    public void RemoveCrown()
    {
        crown.SetActive(false);
        Vector3 hb_pos = health_bar.transform.position;
        health_bar.transform.position = new Vector3(hb_pos.x, hb_pos.y - 0.25f, hb_pos.z);
        Vector3 mb_pos = mana_bar.transform.position;
        mana_bar.transform.position = new Vector3(mb_pos.x, mb_pos.y - 0.25f, mb_pos.z);
        Vector3 bm_pos = behavior_menu.transform.position;
        behavior_menu.transform.position = new Vector3(mb_pos.x, mb_pos.y - 0.25f, mb_pos.z);
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
        left_eye.GetComponent<MeshRenderer>().material = offense_mat;
        right_eye.GetComponent<MeshRenderer>().material = offense_mat;
    }

    public void NeutralBehavior()
    {
        allyEntity.SetBehavior(Entity.Behavior.Neutral);
        left_eye.GetComponent<MeshRenderer>().material = neutral_mat;
        right_eye.GetComponent<MeshRenderer>().material = neutral_mat;
    }

    public void DefenseBehavior()
    {
        allyEntity.SetBehavior(Entity.Behavior.Defense);
        left_eye.GetComponent<MeshRenderer>().material = defense_mat;
        right_eye.GetComponent<MeshRenderer>().material = defense_mat;
    }

    public void SetKing()
    {
        game_manager.SetAllyKing(gameObject);
    }

    public void Sell()
    {
        unit_placement.SellEntity(gameObject);
    }

    public void SetItemEquipped(string item) 
    {
        if (item_visual != null)
        {
            Destroy(item_visual);
        }
        item_equipped = item;
        switch (item)
        {
            case "HealHalo":
                item_visual = Instantiate(heal_halo, transform.position, transform.rotation, transform);
                break;
            case "PowerBelt":
                item_visual = Instantiate(power_belt, transform.position, transform.rotation, transform);
                gameObject.GetComponent<AbilityManager>().abilityMultiplier *= DataHolder.GetBeltAbilityMult();
                break;
            case "PoisonVial":
                item_visual = Instantiate(poison_vial, transform.position, transform.rotation, transform);
                break;
            case "RevengeMask":
                item_visual = Instantiate(revenge_mask, transform.position, transform.rotation, transform);
                break;
            case "ResurrectionEarring":
                item_visual = Instantiate(resurrection_earring, transform.position, transform.rotation, transform);
                break;
            default:
                break;
        }
    }

    public void UnequipItem()
    {
        switch (item_equipped)
        {
            case "PowerBelt":
                gameObject.GetComponent<AbilityManager>().abilityMultiplier /= DataHolder.GetBeltAbilityMult();
                break;
            case "PoisonVial":
                break;
            case "RevengeMask":
                break;
            default:
                break;
        }
        if (item_equipped != "")
        {
            item_equipped = "";
            Destroy(item_visual);
        }
    }

    public string GetItemEquipped() {  return item_equipped; }

    public void SetAlliesOnStart(int allies) { allies_on_start = allies; }
    public int GetAlliesOnStart() { return allies_on_start; }

    public void Dies()
    {
        game_manager.EntityDied();
    }

    public void UpdateRevengeMultiplier()
    {
        Debug.Log("URM");
        if (item_equipped == "RevengeMask")
        {
            Debug.Log("correct item");
            allyEntity.RemoveAttackMultiplier(revenge_mult);
            allyEntity.RemoveDefMultiplier(revenge_mult);
            allyAbility.abilityMultiplier /= (revenge_mult / 2f + 0.5f);
            revenge_mult = 1f + DataHolder.GetMaskRevengeBoost() * Mathf.Max(allies_on_start - game_manager.ally_list.Count, 0);
            allyEntity.AddAttackMultiplier(revenge_mult);
            allyEntity.AddDefMultiplier(revenge_mult);
            allyAbility.abilityMultiplier *= (revenge_mult / 2f + 0.5f);

            Debug.Log("URM no crash :)");
        }
    }

}

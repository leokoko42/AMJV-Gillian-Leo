using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Material normalWarriorMaterial, rageWarriorMaterial;
    [SerializeField] private GameObject archerAbilityBullet;
    private EntityBehavior attackEntityBehavior;
    private BasicEntity attackerEntity;
    private LayerMask entityLayerMask;
    private List<GameObject> summonnedEntitiesList;
    public float abilityMultiplier = 1f;
    private void Start() {
        entityLayerMask = LayerMask.GetMask("Entity");
        attackEntityBehavior = GetComponent<EntityBehavior>();
        attackerEntity = GetComponent<BasicEntity>();
    }
    public bool ActivateAbility(bool abilityAvailable, EntityBehavior.Ability ability, GameObject attacker, GameObject target, bool isTargetInRange) {
        if (!abilityAvailable) {return false;}

        switch (ability) {
            case EntityBehavior.Ability.Warrior:
                return WarriorAbility(attacker);
            case EntityBehavior.Ability.Archer:
                return ArcherAbility(attacker, target, isTargetInRange);
            case EntityBehavior.Ability.Tank:
                return TankAbility(attacker);
            case EntityBehavior.Ability.Hammer:
                return HammerAbility(attacker, target, isTargetInRange);
            case EntityBehavior.Ability.Healer:
                return HealerAbility(attacker);
            case EntityBehavior.Ability.Shaman:
                return ShamanAbility(attacker, target, isTargetInRange);
            case EntityBehavior.Ability.Summoner:
                return SummonerAbility(attacker);
            case EntityBehavior.Ability.Monk:
                return MonkAbility(attacker);
        }

        return false;
    }

    private bool WarriorAbility(GameObject attacker) {
        StartCoroutine(WarriorBoost(attackerEntity));
        return true;
    }
    private IEnumerator WarriorBoost(BasicEntity attackerEntity) {
        MeshRenderer warriorRenderer = GetComponent<MeshRenderer>();
        float globalMultiplier = 1f + 0.3f * abilityMultiplier;

        warriorRenderer.material = rageWarriorMaterial;
        attackerEntity.AddAttackMultiplier(globalMultiplier);
        attackerEntity.AddSpeedMultiplier(globalMultiplier);
        attackerEntity.AddCooldownMultiplier(1f/globalMultiplier);
        yield return new WaitForSeconds(6);
        warriorRenderer.material = normalWarriorMaterial;
        attackerEntity.RemoveAttackMultiplier(globalMultiplier);
        attackerEntity.RemoveSpeedMultiplier(globalMultiplier);
        attackerEntity.RemoveCooldownMultiplier(1/globalMultiplier);
    }

    public bool TankAbility(GameObject attacker) {
        int i=0;
        bool attackerEnemy = attackerEntity.GetIsEnemy();
        Vector3 delta = new Vector3(0, 1, 0);
        Collider[] colliders = Physics.OverlapCapsule(attacker.transform.position - delta, attacker.transform.position + delta, 4f, entityLayerMask);
        foreach (Collider collider in colliders) {
            if (attacker.tag == collider.gameObject.tag) {
                i++;
                BasicEntity targetEntity = collider.GetComponent<BasicEntity>();
                StartCoroutine(TankBoost(targetEntity));
            }
            if (i==4) {
                return true;
            }
        }
        return true;
    }
    private IEnumerator TankBoost(BasicEntity targetedEntity) {
        targetedEntity.SetDef(targetedEntity.GetTrueDef() + 5 * abilityMultiplier);
        yield return new WaitForSeconds(6);
        targetedEntity.SetDef(targetedEntity.GetTrueDef() - 5 * abilityMultiplier);
    }

    public bool HealerAbility(GameObject attacker) {
        Vector3 delta = new Vector3(0, 1, 0);
        Collider[] colliders = Physics.OverlapCapsule(attacker.transform.position - delta, attacker.transform.position + delta, 6f, entityLayerMask);
        foreach (Collider collider in colliders) {
            if (attacker.tag == collider.gameObject.tag) {
                collider.GetComponent<HealthManager>().Heal(30 * abilityMultiplier);
            }
        }
        return true;
    }

    public bool SummonerAbility(GameObject attacker) {
        return true;
    }

    public void AddSummonnedInstance(GameObject summonned) {
        summonnedEntitiesList.Add(summonned);
    }

    public void RemoveSummonnedInstance(GameObject summonned) {
        summonnedEntitiesList.Remove(summonned);
    }
    
    public bool MonkAbility(GameObject attacker) {
        StartCoroutine(MonkBoost(attackerEntity));
        return true;
    }
    private IEnumerator MonkBoost(BasicEntity monkEntity) {
        float totalUpgrade = 10f * abilityMultiplier;
        monkEntity.AddCooldownMultiplier(1/totalUpgrade);
        StopCoroutine(attackEntityBehavior.AttackCooldown());
        yield return new WaitForSeconds(1);
        monkEntity.RemoveCooldownMultiplier(1/totalUpgrade);
    }

    private bool ArcherAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        else {
            float attack = attackerEntity.GetAttack() * abilityMultiplier;
            StartCoroutine(ArcherAbilityBullets(attacker,target,attack));
        }
        return true;
    }
    private IEnumerator ArcherAbilityBullets(GameObject attacker, GameObject target, float attack) {
        for (int i=0; i<7; i++) {
            Vector3 center = target.transform.position;
            float theta = UnityEngine.Random.value * Mathf.PI;
            float r = UnityEngine.Random.value * 2;
            float y = UnityEngine.Random.value;
            Vector3 delta = (new Vector3(r*Mathf.Cos(theta), 10+y, r*Mathf.Sin(theta)));
            GameObject bullet = Instantiate(archerAbilityBullet, center+delta, new Quaternion(90,0,0,0));
            bullet.GetComponent<BulletMovment>().Init(attack, attacker);
            yield return new WaitForSeconds(0.2f);
        }
    }
    public bool HammerAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        Vector3 delta = new Vector3(0, 1, 0);
        Collider[] colliders = Physics.OverlapCapsule(attacker.transform.position - delta, attacker.transform.position + delta, 6f, entityLayerMask);
        foreach (Collider collider in colliders) {
            if (attacker.tag != collider.gameObject.tag) {
                collider.GetComponent<HealthManager>().Damage(attackerEntity.GetAttack() * abilityMultiplier);
                collider.GetComponent<EntityBehavior>().ApplyKnockback(transform.position, 3, 10 * abilityMultiplier);
            }
        }
        return true;
    }

    public bool ShamanAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        Vector3 delta = new Vector3(0, 1, 0);
        Collider[] colliders = Physics.OverlapCapsule(attacker.transform.position - delta, attacker.transform.position + delta, 6f, entityLayerMask);
        foreach (Collider collider in colliders) {
            if (attacker.tag != collider.gameObject.tag) {
                EntityBehavior targetBehavior = collider.GetComponent<EntityBehavior>();
                int randomStatus = UnityEngine.Random.Range(0,3);
                switch (randomStatus) {
                    case 0:
                        targetBehavior.ApplyFire(8 * abilityMultiplier);
                        break;
                    case 1:
                        targetBehavior.ApplyPoison(8 * abilityMultiplier);
                        break;
                    case 2:
                        targetBehavior.ApplyIce(8 * abilityMultiplier);
                        break;
                    default:
                        throw new Exception("Wrong Status effect from Shaman");
                }
            }
        }
        return true;
    }
}

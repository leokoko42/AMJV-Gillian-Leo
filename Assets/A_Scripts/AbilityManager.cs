using System.Collections;
using UnityEngine;
public class AbilityManager : MonoBehaviour
{
    [SerializeField] private Material normalWarriorMaterial, rageWarriorMaterial;
    public float abilityMultiplier = 1f;
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
        BasicEntity attackerEntity = attacker.GetComponent<BasicEntity>();
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
        Debug.Log("StartBuff");
        yield return new WaitForSeconds(6);
        warriorRenderer.material = normalWarriorMaterial;
        attackerEntity.RemoveAttackMultiplier(globalMultiplier);
        attackerEntity.RemoveSpeedMultiplier(globalMultiplier);
        attackerEntity.RemoveCooldownMultiplier(1/globalMultiplier);
        Debug.Log("EndBuff");
    }

    public bool TankAbility(GameObject attacker) {
        return true;
    }

    public bool HealerAbility(GameObject attacker) {
        return true;
    }

    public bool SummonerAbility(GameObject attacker) {
        return true;
    }
    
    public bool MonkAbility(GameObject attacker) {
        return true;
    }

    private bool ArcherAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        return true;
    }
    public bool HammerAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        return true;
    }

    public bool ShamanAbility(GameObject attacker, GameObject target, bool isEnemyInRange) {
        if (!isEnemyInRange) {
            return false;
        }
        return true;
    }
}

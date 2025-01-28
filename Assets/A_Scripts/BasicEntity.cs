using System.Collections.Generic;
using UnityEngine;

public class BasicEntity : Entity
{
    [SerializeField] protected float manaRegen;

    public override float GetAttack()
    {
        return this.attack * GetAttackMultiplier();
    }

    public override float GetAttackSpeed()
    {
        return this.attackSpeed * GetCooldownMultiplier();
    }

    public override Behavior GetBehavior()
    {
        return this.behavior;
    }

    public override int GetCost()
    {
        return this.cost;
    }

    public override float GetDef()
    {
        return this.def * GetDefMultiplier();
    }
    public float GetTrueDef()
    {
        return this.def;
    }

    public override float GetHP()
    {
        return this.hp;
    }

    public override bool GetIsActive()
    {
        return this.isActive;
    }

    public override bool GetIsEnemy()
    {
        return this.isEnemy;
    }

    public override bool GetIsKing()
    {
        return this.isKing;
    }

    public override float GetMana()
    {
        return this.mana;
    }

    public override float GetMaxHP()
    {
        return this.maxHP;
    }

    public override float GetMaxMana()
    {
        return this.maxMana;
    }

    public override float GetRange()
    {
        return this.range;
    }

    public override float GetSpeed()
    {
        return this.speed * GetSpeedMultiplier();
    }
    public float GetManaRegen()
    {
        return this.manaRegen;
    }


    public override void SetAttack(float newAttack)
    {
        this.attack = newAttack;
    }

    public override void SetAttackSpeed(float newAttackSpeed)
    {
        this.attackSpeed = newAttackSpeed;
    }

    public override void SetBehavior(Behavior newBehavior)
    {
        this.behavior = newBehavior;
    }

    public override void SetCost(int newCost)
    {
        this.cost = newCost;
    }

    public override void SetDef(float newDef)
    {
        this.def = newDef;
    }

    public override void SetHP(float newHP)
    {
        this.hp = newHP;
    }

    public override void SetIsActive(bool newIsActive)
    {
        this.isActive = newIsActive;
    }

    public override void SetIsEnemy(bool newIsEnemy)
    {
        this.isEnemy = newIsEnemy;
    }

    public override void SetIsKing(bool newIsKing)
    {
        this.isKing = newIsKing;
    }

    public override void SetMana(float newMana)
    {
        this.mana = newMana;
    }

    public override void SetMaxHP(float newMaxHP)
    {
        this.maxHP = newMaxHP;
    }

    public override void SetMaxMana(float newMaxMana)
    {
        this.maxMana = newMaxMana;
    }

    public override void SetRange(float newRange)
    {
        this.range = newRange;
    }

    public override void SetSpeed(float newSpeed)
    {
        this.speed = newSpeed;
    }
    public void SetManaRegen(float newManaRegen)
    {
        this.manaRegen = newManaRegen;
    }


    private List<float> attackMultiplier = new List<float>();
    public List<float> defMultiplier = new List<float>();
    private List<float> speedMultiplier = new List<float>();
    private List<float> cooldownMultiplier = new List<float>();


    public void AddAttackMultiplier(float multiplier)
    {
        this.attackMultiplier.Add(multiplier);
    }
    public void AddDefMultiplier(float multiplier)
    {
        this.defMultiplier.Add(multiplier);
    }
    public void AddSpeedMultiplier(float multiplier)
    {
        this.speedMultiplier.Add(multiplier);
    }
    public void AddCooldownMultiplier(float multiplier)
    {
        this.cooldownMultiplier.Add(multiplier);
    }

    public void RemoveAttackMultiplier(float multiplier)
    {
        this.attackMultiplier.Remove(multiplier);
    }
    public void RemoveDefMultiplier(float multiplier)
    {
        this.defMultiplier.Remove(multiplier);
    }
    public void RemoveSpeedMultiplier(float multiplier)
    {
        this.speedMultiplier.Remove(multiplier);
    }
    public void RemoveCooldownMultiplier(float multiplier)
    {
        this.cooldownMultiplier.Remove(multiplier);
    }

    private float GetAttackMultiplier() {
        float toatalMultiplier = 1f;
        foreach (float multiplier in this.attackMultiplier) {
            toatalMultiplier *= multiplier;
        }
        return toatalMultiplier;
    }
    private float GetDefMultiplier() {
        float toatalMultiplier = 1f;
        foreach (float multiplier in this.defMultiplier) {
            toatalMultiplier *= multiplier;
        }
        return toatalMultiplier;
    }
    private float GetSpeedMultiplier() {
        float toatalMultiplier = 1f;
        foreach (float multiplier in this.speedMultiplier) {
            toatalMultiplier *= multiplier;
        }
        return toatalMultiplier;
    }
    private float GetCooldownMultiplier() {
        float toatalMultiplier = 1f;
        foreach (float multiplier in this.cooldownMultiplier) {
            toatalMultiplier *= multiplier;
        }
        return toatalMultiplier;
    }

}

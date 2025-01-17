using UnityEngine;

public class BasicEntity : Entity
{
    [SerializeField] protected float manaRegen;

    public override float GetAttack()
    {
        return this.attack;
    }

    public override float GetAttackSpeed()
    {
        return this.attackSpeed;
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
        return this.speed;
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using UnityEngine;

public class BasicEntity : Entity
{
    

    public override int GetAttack()
    {
        return this.attack;
    }

    public override int GetAttackSpeed()
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

    public override int GetDef()
    {
        return this.def;
    }

    public override int GetHP()
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

    public override int GetMana()
    {
        return this.mana;
    }

    public override int GetMaxHP()
    {
        return this.maxHP;
    }

    public override int GetMaxMana()
    {
        return this.maxMana;
    }

    public override float GetRange()
    {
        return this.range;
    }

    public override int GetSpeed()
    {
        return this.speed;
    }

    public override void SetAttack(int newAttack)
    {
        this.attack = newAttack;
    }

    public override void SetAttackSpeed(int newAttackSpeed)
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

    public override void SetDef(int newDef)
    {
        this.def = newDef;
    }

    public override void SetHP(int newHP)
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

    public override void SetMana(int newMana)
    {
        this.mana = newMana;
    }

    public override void SetMaxHP(int newMaxHP)
    {
        this.maxHP = newMaxHP;
    }

    public override void SetMaxMana(int newMaxMana)
    {
        this.maxMana = newMaxMana;
    }

    public override void SetRange(int newRange)
    {
        this.range = newRange;
    }

    public override void SetSpeed(int newSpeed)
    {
        this.speed = newSpeed;
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

using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected bool isActive;
    [SerializeField] protected bool isEnemy;
    [SerializeField] protected bool isKing;
    [SerializeField] public enum Behavior {Neutral, Offense, Defense};
    [SerializeField] public Behavior behavior;

    [SerializeField] protected float hp;
    [SerializeField] protected float maxHP;
    [SerializeField] protected float def;
    [SerializeField] protected float speed;
    [SerializeField] protected float attackSpeed;
    [SerializeField] protected float attack;
    [SerializeField] protected float mana;
    [SerializeField] protected float maxMana;
    [SerializeField] protected int cost;
    [SerializeField] protected float range;

    public abstract bool GetIsActive(); // Récupère si un personnage est actif ou pas sur la carte (ie il fait rien ou il joue)
    public abstract void SetIsActive(bool newIsActive);
    public abstract bool GetIsEnemy(); // Est ce que l'entity est un ennemi ?
    public abstract void SetIsEnemy(bool newIsEnemy);
    public abstract bool GetIsKing();
    public abstract void SetIsKing(bool newIsKing);

    public abstract Behavior GetBehavior();
    public abstract void SetBehavior(Behavior newBehavior);
   

    // PV
    public abstract float GetHP();
    public abstract void SetHP(float newHP);

    public abstract float GetMaxHP();
    public abstract void SetMaxHP(float newMaxHP);  

    // Défense
    public abstract float GetDef();
    public abstract void SetDef(float newDef);

    // Vitesse
    public abstract float GetSpeed(); // La vitesse de déplacement
    public abstract void SetSpeed(float newSpeed);

    // Attaque
    public abstract float GetAttackSpeed();
    public abstract void SetAttackSpeed(float newAttackSpeed);
    public abstract float GetAttack();
    public abstract void SetAttack(float newAttack);

    // Mana
    public abstract float GetMana();
    public abstract void SetMana(float newMana);
    public abstract float GetMaxMana();
    public abstract void SetMaxMana(float newMaxMana);

    // Coût
    public abstract int GetCost(); // Le coût à payer pour placer l'entity sur l'arène
    public abstract void SetCost(int newCost);

    // Portée
    public abstract float GetRange(); 
    public abstract void SetRange(float newRange);

    // Pour les capacités spéciales : Les différentes capacités sont des components qui héritent de la classe "Capacité".
    // => Pour donner une capacité spéciale à une unité il faut lui donner le component (script) qui correspond à la capacité spéciale choisie

    // Les classes "HealthManager", les classes d'attaques, etc vont dépendre de la classe des personnages (qui héritent de "Entity")



}
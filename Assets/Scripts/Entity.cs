using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [SerializeField] protected bool isActive;
    [SerializeField] protected bool isEnemy;
    [SerializeField] protected bool isKing;
    [SerializeField] public enum Behavior {Neutral, Offense, Defense};

    [SerializeField] protected int hp;
    [SerializeField] protected int maxHP;
    [SerializeField] protected int def;
    [SerializeField] protected int speed;
    [SerializeField] protected int attackSpeed;
    [SerializeField] protected int attack;
    [SerializeField] protected int mana;
    [SerializeField] protected int maxMana;
    [SerializeField] protected int cost;

    public abstract bool GetIsActive(); // Récupère si un personnage est actif ou pas sur la carte (ie il fait rien ou il joue)
    public abstract void SetIsActive(bool newIsActive);
    public abstract bool GetIsEnemy(); // Est ce que l'entity est un ennemi ?
    public abstract void SetIsEnemy(bool newIsEnemy);
    public abstract bool GetIsKing();
    public abstract void SetIsKing(bool newIsKing);

    public abstract Behavior GetBehavior();
    public abstract void SetBehavior(Behavior newBehavior);
   

    // PV
    public abstract int GetHP();
    public abstract void SetHP(int newHP);

    public abstract int GetMaxHP();
    public abstract void SetMaxHP(int newMaxHP);  

    // Défense
    public abstract int GetDef();
    public abstract void SetDef(int newDef);

    // Vitesse
    public abstract int GetSpeed(); // La vitesse de déplacement
    public abstract void SetSpeed(int newSpeed);

    // Attaque
    public abstract int GetAttackSpeed();
    public abstract void SetAttackSpeed(int newAttackSpeed);
    public abstract int GetAttack();
    public abstract void SetAttack(int newAttack);

    // Mana
    public abstract int GetMana();
    public abstract void SetMana(int newMana);
    public abstract int GetMaxMana();
    public abstract void SetMaxMana(int newMaxMana);

    // Coût
    public abstract int GetCost(); // Le coût à payer pour placer l'entity sur l'arène
    public abstract void SetCost();

    // Pour les capacités spéciales : Les différentes capacités sont des components qui héritent de la classe "Capacité".
    // => Pour donner une capacité spéciale à une unité il faut lui donner le component (script) qui correspond à la capacité spéciale choisie

    // Les classes "HealthManager", les classes d'attaques, etc vont dépendre de la classe des personnages (qui héritent de "Entity")



}

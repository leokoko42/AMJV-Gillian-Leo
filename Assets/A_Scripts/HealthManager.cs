using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
public class HealthManager : MonoBehaviour
{
    private GameManager gameManager;
    BasicEntity entity;
    public UnityEvent<float,float> health_change;
    public UnityEvent<float> damaged;
    public UnityEvent<float> damage_absorbed;
    public UnityEvent<float> healed;
    public UnityEvent death;
    private LayerMask layerMask;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        entity = GetComponent<BasicEntity>();
        layerMask = LayerMask.GetMask("Entity");
    }

    // Update is called once per frame
    private bool hPressed;
    void Update() {

    }

    void FixedUpdate() {
        if (transform.position.y < -50) {
            Death();
        }
    }

    public void Heal(float healAmount) {
        float entityHp = entity.GetHP();
        float entityMaxHp = entity.GetMaxHP();
        float newHp = Mathf.Min(entityMaxHp, entityHp + healAmount);
        entity.SetHP(newHp);
        health_change.Invoke(newHp, entityMaxHp);
        healed.Invoke(newHp-entityHp);
    }

    public float Damage(float damageAmount)
    {
        float entityHp = entity.GetHP();
        float absorbed = Mathf.Min(damageAmount, entity.GetDef());
        damageAmount -= absorbed;
        float newHp = entityHp - damageAmount;
        if (newHp <= 0) {
            Death();
            entity.SetHP(newHp);
        }
        else {
            entity.SetHP(newHp);
        }
        health_change.Invoke(newHp, entity.GetMaxHP());
        damaged.Invoke(entityHp - newHp);
        damage_absorbed.Invoke(absorbed);
        return damageAmount;
    }

    public float TrueDamage(float damageAmount) {
        float entityHp = entity.GetHP();
        float newHp = entityHp - damageAmount;
        if (newHp <= 0) {
            Death();
        }
        else {
            entity.SetHP(newHp);
        }
        health_change.Invoke(newHp, entity.GetMaxHP());
        damaged.Invoke(entityHp - newHp);
        damage_absorbed.Invoke(damageAmount);
        return damageAmount;
    }

    public void Death() {
        EntityBehavior eb = gameObject.GetComponent<EntityBehavior>();
        if (eb.GetItemEquipped() == "ResurrectionEarring")
        {
            eb.UnequipItem();
            gameObject.transform.localScale *= 0.75f;
            float reduction_factor = DataHolder.GetEarringReductionFactor();
            entity.SetMaxHP(entity.GetMaxHP() / reduction_factor);
            entity.SetHP(entity.GetMaxHP());
            entity.SetAttack(entity.GetAttack() / (reduction_factor/2 + 0.5f));
            entity.SetDef(entity.GetDef() / (reduction_factor / 2 + 0.5f));
        }
        else
        {
            death.Invoke();
            if (GetComponent<BasicEntity>().GetIsEnemy())
            {
                gameManager.RemoveEnemy(gameObject);
            }
            else
            {
                gameManager.RemoveAlly(gameObject);
            }
            Destroy(gameObject);
        }
       
    }

}

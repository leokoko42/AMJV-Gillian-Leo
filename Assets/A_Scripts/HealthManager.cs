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
        if (transform.position.y < -10) {
            Death();
        }
    }

    public void Heal(float healAmount) {
        float entityHp = entity.GetHP();
        float entityMaxHp = entity.GetMaxHP();
        float newHp = Mathf.Min(entityMaxHp, entityHp + healAmount);
        health_change.Invoke(newHp, entityMaxHp);
        healed.Invoke(newHp-entityHp);
    }

    public float Damage(float damageAmount) {
        float entityHp = entity.GetHP();
        float absorbed = Mathf.Min(damageAmount, entity.GetDef());
        damageAmount -= absorbed;
        float newHp = entityHp - damageAmount;
        if (newHp <= 0) {
            Death();
        }
        else {
            entity.SetHP(newHp);
        }
        health_change.Invoke(newHp, entity.GetMaxHP());
        damaged.Invoke(entityHp - newHp);
        damage_absorbed.Invoke(absorbed);
        return damageAmount;
    }

    void Death() {
        Vector3 delta = new Vector3(0,1,0);
        Collider[] colliders = Physics.OverlapCapsule(transform.position - delta, transform.position + delta, 5f, layerMask);
        foreach (Collider collider in colliders) {
            collider.GetComponent<EntityBehavior>().ApplyKnockback(transform.position, 3);
        }

        if (GetComponent<BasicEntity>().GetIsEnemy()) {
            gameManager.RemoveEnemy(gameObject);
        }
        else {
            gameManager.RemoveAlly(gameObject);
        }
        Destroy(gameObject);
    }

}

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Entity))]
public class HealthManager : MonoBehaviour
{
    BasicEntity entity;
    public UnityEvent<float,float> health_change;
    public UnityEvent<float> damaged;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        entity = GetComponent<BasicEntity>();
    }

    // Update is called once per frame
    private bool hPressed;
    void Update()
    {
        hPressed = Input.GetKeyDown(KeyCode.H);
    }

    void FixedUpdate() {
        if (hPressed) {
            Damage(50);
        }
    }

    public void Heal(int healAmount) {
        int entityHp = entity.GetHP();
        int entityMaxHp = entity.GetMaxHP();
        int newHp = entityHp + healAmount;
        if (newHp >= entityMaxHp) {
            entity.SetHP(entityMaxHp);
        }
        else {
            entity.SetHP(newHp);
        }
        health_change.Invoke(newHp, entityMaxHp);
    }

    public void Damage(int damageAmount) {
        int entityHp = entity.GetHP();
        int newHp = entityHp - damageAmount;
        if (newHp <= 0) {
            Death();
        }
        else {
            entity.SetHP(newHp);
        }
        health_change.Invoke(newHp, entity.GetMaxHP());
        damaged.Invoke(entityHp - newHp);
    }

    void Death() {
        Destroy(gameObject);
    }

}

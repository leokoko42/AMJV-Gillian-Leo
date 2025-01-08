using UnityEngine;

[RequireComponent(typeof(Entity))]
public class HealthManager : MonoBehaviour
{
    BasicEntity entity;
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
    }

    void Death() {
        Destroy(gameObject);
    }

}

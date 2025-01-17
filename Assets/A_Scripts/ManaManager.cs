using UnityEngine;
using UnityEngine.Events;

public class ManaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    BasicEntity entity;
    public UnityEvent<float, float> mana_change;
    public UnityEvent<float> mana_gain;
    private float mana_regen_timer;
    void Start()
    {
        entity = GetComponent<BasicEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        float mana_delay = 1f / entity.GetManaRegen();
        if (mana_regen_timer > mana_delay)
        {
            addMana(1);
            mana_regen_timer -= mana_delay;
        }
        mana_regen_timer += Time.deltaTime;
    }

    public void addMana(float manaAmount) {
        float entityMana = entity.GetMana();
        float entityMaxMana = entity.GetMaxMana();
        float newMana = Mathf.Min(entityMaxMana, entityMana + manaAmount);
        mana_change.Invoke(newMana, entityMaxMana);
        mana_gain.Invoke(newMana - entityMana);
    }

    public void RemoveMana(float manaAmount) {
        float entityMana = entity.GetMana();
        float newMana = entityMana - manaAmount;
        if (newMana <= 0) {
            entity.SetMana(0);
        }
        else {
            entity.SetMana(newMana);
        }
        mana_change.Invoke(newMana, entity.GetMaxMana());
    }

    public void ManaOnDamageRecieved(float value)
    {
        addMana(value / 2);
    }

    public void ManaOnDamageDealt(float value)
    {
        addMana(value / 3);
    }
}

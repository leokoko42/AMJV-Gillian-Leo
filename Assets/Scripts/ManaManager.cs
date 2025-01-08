using UnityEngine;

public class ManaManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    BasicEntity entity;
    void Start()
    {
        entity = GetComponent<BasicEntity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addMana(int manaAmount) {
        int entityMana = entity.GetMana();
        int entityMaxMana = entity.GetMaxMana();
        int newMana = entityMana + manaAmount;
        if (newMana >= entityMaxMana) {
            entity.SetMana(entityMaxMana);
        }
        else {
            entity.SetMana(newMana);
        }
    }

    public void RemoveMana(int manaAmount) {
        int entityMana = entity.GetMana();
        int newMana = entityMana - manaAmount;
        if (newMana <= 0) {
            entity.SetMana(0);
        }
        else {
            entity.SetMana(newMana);
        }
    }
}

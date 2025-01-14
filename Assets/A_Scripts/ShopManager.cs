using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button warrior;
    [SerializeField] private Button start;
    [SerializeField] private GameObject shop;
    public UnityEvent<string> change_spawn;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        warrior.onClick.AddListener(ChangeToWarrior);
        start.onClick.AddListener(StartBattle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ChangeToWarrior()
    {
        change_spawn.Invoke("Warrior");
    }

    void StartBattle()
    {
        foreach (GameObject ally in gameManager.ally_list)
        {
            BasicEntity ally_behavior = ally.GetComponent<BasicEntity>();
            ally_behavior.SetIsActive(true);
        }
        shop.SetActive(false);
    }
}

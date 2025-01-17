using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem.iOS;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button start;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject button_prefab;
    [SerializeField] private Transform parent_panel;

    public UnityEvent<GameObject> change_spawn;
    private GameObject[] ally_prefabs;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //warrior.onClick.AddListener(ChangeToWarrior);
        //tank.onClick.AddListener(ChangeToTank);
        start.onClick.AddListener(StartBattle);
        ally_prefabs = gameManager.FindPrefabs("Assets/Prefabs/Entities/Allies");
        CreateButtons(ally_prefabs);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CreateButtons(GameObject[] prefabs)
    {
        int n_prefabs = prefabs.Length;
        for (int i = 0; i < n_prefabs; i++)
        {
            // Instantiate a new button from the prefab
            GameObject newButton = Instantiate(button_prefab, parent_panel);

            // Set the button's position (optional, handled by layout components if used)
            newButton.transform.localPosition = new Vector3(0, 370 - 700/(n_prefabs-1) * i, 0);

            // Change the button's text
            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = prefabs[i].name;
            }

            // Add a click event to the button
            Button buttonComponent = newButton.GetComponent<Button>();
            if (buttonComponent != null)
            {
                int index = i;
                buttonComponent.onClick.AddListener(() => ChangeUnit(prefabs[index]));
            }
        }
    }
    void ChangeUnit(GameObject obj)
    {
        change_spawn.Invoke(obj);
    }

    void StartBattle()
    {
        foreach (GameObject ally in gameManager.ally_list)
        {
            BasicEntity ally_behavior = ally.GetComponent<BasicEntity>();
            ally_behavior.SetIsActive(true);
        }
        foreach (GameObject enemy in gameManager.enemy_list)
        {
            BasicEntity enemy_behavior = enemy.GetComponent<BasicEntity>();
            enemy_behavior.SetIsActive(true);
        }
        shop.SetActive(false);
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Button start;
    [SerializeField] private GameObject shop;
    [SerializeField] private GameObject button_prefab;
    [SerializeField] private Transform parent_panel;
    [SerializeField] private Button heal_halo;
    [SerializeField] private Button power_belt;
    [SerializeField] private Button poison_vial;
    [SerializeField] private Button revenge_mask;
    [SerializeField] private Button resurrection_earring;

    public GameObject heal_halo_wielder, power_belt_wielder, poison_vial_wielder, revenge_mask_wielder, resurrection_earring_wielder;

    public UnityEvent<GameObject> change_spawn;
    private GameObject[] ally_prefabs;

    private GameObject selected_entity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.AddListener(StartBattle);
        ally_prefabs = gameManager.FindPrefabs("AllyEntities");
        CreateButtons(ally_prefabs);
        gameManager.SetGameActive(false);
        heal_halo.onClick.AddListener(() => EquipItem("HealHalo"));
        power_belt.onClick.AddListener(() => EquipItem("PowerBelt"));
        poison_vial.onClick.AddListener(() => EquipItem("PoisonVial"));
        revenge_mask.onClick.AddListener(() => EquipItem("RevengeMask"));
        resurrection_earring.onClick.AddListener(() => EquipItem("ResurrectionEarring"));
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
            GameObject newButton = Instantiate(button_prefab, parent_panel);
            RectTransform rectTransform = newButton.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector3(0.5f, 0.88f - (0.88f - 0.2f) / (n_prefabs - 1) * i);
            rectTransform.anchorMax = new Vector3(0.5f, 0.88f - (0.88f - 0.2f) / (n_prefabs - 1) * i);
            rectTransform.anchoredPosition = Vector2.zero;

            TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();
            if (buttonText != null)
            {
                buttonText.text = prefabs[i].name;
            }

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
        if (gameManager.ally_king != null)
        {
            foreach (GameObject ally in gameManager.ally_list)
            {
                BasicEntity ally_behavior = ally.GetComponent<BasicEntity>();
                ally_behavior.SetIsActive(true);
            }
            revenge_mask_wielder.GetComponent<EntityBehavior>().SetAlliesOnStart(gameManager.ally_list.Count);
            foreach (GameObject enemy in gameManager.enemy_list)
            {
                BasicEntity enemy_behavior = enemy.GetComponent<BasicEntity>();
                enemy_behavior.SetIsActive(true);
            }
            DeselectEntity();
            gameManager.SetShopActive(false);
            gameManager.SetGameActive(true);
            shop.SetActive(false);
        }
    }

    public void SetSelectedEntity(GameObject entity)
    {
        DeselectEntity();
        selected_entity = entity;
        selected_entity.GetComponent<EntityBehavior>().ShowMenu();
    }

    public void DeselectEntity()
    {
        if (selected_entity != null) { selected_entity.GetComponent<EntityBehavior>().HideMenu(); }
        selected_entity = null;
    }

    public GameObject GetSelectedEntity() { return selected_entity; }

    private void EquipItem(string item)
    {
        if (selected_entity != null)
        {
            SetItemWielder(item, selected_entity);
        }
    }

    public void SetItemWielder(string item, GameObject w)
    {
        switch (item)
        {
            case "HealHalo":
                if (heal_halo_wielder != null) { Debug.Log("HH"); heal_halo_wielder.GetComponent<EntityBehavior>().SetItemEquipped(""); }
                heal_halo_wielder = w;
                heal_halo_wielder.GetComponent<EntityBehavior>().SetItemEquipped("HealHalo");
                break;
            case "PowerBelt":
                if (power_belt_wielder != null) { power_belt_wielder.GetComponent<EntityBehavior>().SetItemEquipped(""); }
                power_belt_wielder = w;
                power_belt_wielder.GetComponent<EntityBehavior>().SetItemEquipped("PowerBelt");
                break;
            case "PoisonVial":
                if (poison_vial_wielder != null) { poison_vial_wielder.GetComponent<EntityBehavior>().SetItemEquipped(""); }
                poison_vial_wielder = w;
                poison_vial_wielder.GetComponent<EntityBehavior>().SetItemEquipped("PoisonVial");
                break;
            case "RevengeMask":
                if (revenge_mask_wielder != null) { revenge_mask_wielder.GetComponent<EntityBehavior>().SetItemEquipped(""); }
                revenge_mask_wielder = w;
                revenge_mask_wielder.GetComponent<EntityBehavior>().SetItemEquipped("RevengeMask");
                break;
            case "ResurrectionEarring":
                if (resurrection_earring_wielder != null) { resurrection_earring_wielder.GetComponent<EntityBehavior>().SetItemEquipped(""); }
                resurrection_earring_wielder = w;
                resurrection_earring_wielder.GetComponent<EntityBehavior>().SetItemEquipped("ResurrectionEarring");
                break;
            default:
                break;
        }
    }
}

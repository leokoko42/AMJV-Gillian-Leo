using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.EventSystems.EventTrigger;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resume;
    [SerializeField] private Button quit;
    [SerializeField] private Button settings;
    [SerializeField] private TMP_Text round_text;
    [SerializeField] private TMP_Text time_text;
    [SerializeField] private TMP_Text coin_text;
    // Stats menu
    [SerializeField] private GameObject stats_panel;
    [SerializeField] private TMP_Text unit_type;
    [SerializeField] private TMP_Text damage_dealt;
    [SerializeField] private TMP_Text damage_taken;
    [SerializeField] private TMP_Text damage_absorbed;
    [SerializeField] private TMP_Text enemies_defeated;
    [SerializeField] private TMP_Text mana_charged;
    [SerializeField] private TMP_Text abilities_used;
    [SerializeField] private TMP_Text health_recovered;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask layerEntity;
    private GameObject selected_entity;

    public List<GameObject> enemy_list;
    public List<GameObject> ally_list;
    public Dictionary<string, int> entity_max_occurences;
    public GameObject ally_king;

    public int coins;
    private float start_time;

    private bool shop_active;
    private bool game_active;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resume.onClick.AddListener(TogglePause);
        quit.onClick.AddListener(QuitGame);
        settings.onClick.AddListener(SettingsMenu);
        start_time = Time.time;
        GameObject[] enemy_array = GameObject.FindGameObjectsWithTag("Enemies");
        GameObject[] ally_array = GameObject.FindGameObjectsWithTag("Allies");
        enemy_list = new List<GameObject>(enemy_array);
        ally_list = new List<GameObject>(ally_array);
        coins = 100;
        //entity_max_occurences = new() { { "Archer", 3 }, { "Hammer", 3 }, { "Healer", 3 }, { "Monk", 3 }, { "Summoner", 3 }, { "Tank", 3 }, { "Warrior", 3 }, { "Wololo", 3 } };

        stats_panel.SetActive(false);
        pauseMenu.SetActive(false);

        shop_active = true;
        game_active = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        UpdateTime();

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerEntity))
            {
                if (hit.collider != null && !coin_text.isActiveAndEnabled)
                {
                    stats_panel.SetActive(true);
                    selected_entity = hit.transform.gameObject;
                }
            }
        }
        if (stats_panel.activeSelf) { UpdateStats(); }
            
    }

    void UpdateStats()
    {
        EntityBehavior behavior = selected_entity.GetComponent<EntityBehavior>();
        unit_type.text = selected_entity.name.Replace("(Clone)", "") + " Unit";
        damage_dealt.text = "Damage Dealt : " + Mathf.Round(behavior.total_damage_dealt).ToString();
        damage_taken.text = "Damage Taken : " + Mathf.Round(behavior.total_damage_taken).ToString();
        damage_absorbed.text = "Damage Absorbed : " + Mathf.Round(behavior.total_damage_absorbed).ToString();
        enemies_defeated.text = "Enemies Defeated : " + Mathf.Round(behavior.total_enemies_defeated).ToString();
        mana_charged.text = "Mana Charged : " + Mathf.Round(behavior.total_mana_charged).ToString();
        abilities_used.text = "Abilities Used : " + Mathf.Round(behavior.total_abilities_used).ToString();
        health_recovered.text = "Health Recovered : " + Mathf.Round(behavior.total_health_recovered).ToString();
    }

    void UpdateTime()
    {
        int time_elapsed = (int)(Time.time - start_time);
        int seconds = time_elapsed % 60;
        int minutes = (time_elapsed / 60) % 60;
        int hours = time_elapsed / 3600;
        if (hours > 0)
        {
            time_text.text = string.Format("Time : {0}h{1}m{2}s", hours, minutes, seconds);
        }
        else if (minutes > 0)
        {
            time_text.text = string.Format("Time : {0}m{1}s", minutes, seconds);
        }
        else
        {
            time_text.text = string.Format("Time : {0}s", seconds);
        }
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        coin_text.text = coins.ToString();
    }

    public void RemoveCoins(int amount)
    {
        coins -= amount;
        coin_text.text = coins.ToString();
    }

    void TogglePause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
    }

    void SettingsMenu()
    {

    }

    void QuitGame()
    {
        Application.Quit();
    }

    public void AddEnemy(GameObject e)
    {
        enemy_list.Add(e);
    }

    public void AddAlly(GameObject a)
    {
        ally_list.Add(a);
    }

    public void RemoveEnemy(GameObject e)
    {
        enemy_list.Remove(e);
    }

    public void RemoveAlly(GameObject a)
    {
        ally_list.Remove(a);
    }

    public void SetAllyKing(GameObject k)
    {
        if (ally_king != null) 
        { 
            ally_king.GetComponent<BasicEntity>().SetIsKing(false);
            ally_king.GetComponent<EntityBehavior>().RemoveCrown();
            ally_king = null;
        }
        ally_king = k;
        k.GetComponent<BasicEntity>().SetIsKing(true);
        k.GetComponent<EntityBehavior>().AddCrown();
    }

    public GameObject[] FindPrefabs(string folderPath)
    {

        // Get all asset paths in the specified folder
        string[] assetPaths = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        // List to store the loaded prefabs
        List<GameObject> prefabs = new List<GameObject>();

        foreach (string guid in assetPaths)
        {
            // Get the full path to the asset
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);

            // Load the prefab and add it to the list
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
            if (prefab != null)
            {
                prefabs.Add(prefab);
            }
        }

        return prefabs.ToArray();
    }

    public bool GetShopActive()
    {
        return shop_active;
    }

    public void SetShopActive(bool b)
    {
        shop_active = b;
    }

    public bool GetGameActive()
    {
        return game_active;
    }

    public void SetGameActive(bool b)
    {
        game_active = b;
    }
}

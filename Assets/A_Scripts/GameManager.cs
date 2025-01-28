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
    [SerializeField] private GameObject gameUI;
    [SerializeField] private TMP_Text round_text;
    [SerializeField] private TMP_Text time_text;
    [SerializeField] private TMP_Text enemies_text;
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

    [SerializeField] private ShopManager shopManager;

    private bool shop_active;
    private bool game_active;

    [SerializeField] private GameObject victoryScreen;
    [SerializeField] private Button next_level_button;
    [SerializeField] private Button victory_quit_button;

    [SerializeField] private GameObject uiButtons;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject pauseText;

    public List<GameObject> enemy_list;
    public List<GameObject> ally_list;
    public Dictionary<string, int> entity_max_occurences;
    public GameObject ally_king;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject[] enemy_array = GameObject.FindGameObjectsWithTag("Enemies");
        GameObject[] ally_array = GameObject.FindGameObjectsWithTag("Allies");
        enemy_list = new List<GameObject>(enemy_array);
        ally_list = new List<GameObject>(ally_array);

        stats_panel.SetActive(false);

        shop_active = true;
        game_active = false;

        pauseButton.onClick.AddListener(PauseOrResume);
        settingsButton.onClick.AddListener(SettingsMenu);
        quitButton.onClick.AddListener(QuitGame);
        pauseText.SetActive(false);
        next_level_button.onClick.AddListener(NextScene);
        victory_quit_button.onClick.AddListener(QuitGame);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseOrResume();
        }
        UpdateTime();
        enemies_text.text = "Enemies Left : " + enemy_list.Count.ToString();

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
        if (enemy_list.Count == 0)
        {
            Victory();
        }
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
        int time_elapsed = (int)(Time.time - DataHolder.GetStartTime());
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
        DataHolder.SetCoins(DataHolder.GetCoins() + amount);
        coin_text.text = DataHolder.GetCoins().ToString();
    }

    public void RemoveCoins(int amount)
    {
        DataHolder.SetCoins(DataHolder.GetCoins() - amount);
        coin_text.text = DataHolder.GetCoins().ToString();
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
        // Old Code : Works only in Editor
        //// Get all asset paths in the specified folder
        //string[] assetPaths = AssetDatabase.FindAssets("t:Prefab", new[] { folderPath });

        //// List to store the loaded prefabs
        //List<GameObject> prefabs = new List<GameObject>();

        //foreach (string guid in assetPaths)
        //{
        //    // Get the full path to the asset
        //    string assetPath = AssetDatabase.GUIDToAssetPath(guid);

        //    // Load the prefab and add it to the list
        //    GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        //    if (prefab != null)
        //    {
        //        prefabs.Add(prefab);
        //    }
        //}
        //return prefabs.ToArray();

        Object[] prefabs = Resources.LoadAll(folderPath, typeof(GameObject));

        if (prefabs.Length == 0)
        {
            Debug.LogWarning($"No prefabs found in Resources/{folderPath}");
        }

        List<GameObject> list = new List<GameObject>();
        foreach (Object obj in prefabs)
        {
            GameObject go = (GameObject)obj;
            list.Add(go);
        }
        return list.ToArray();
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
        uiButtons.SetActive(b);
        gameUI.SetActive(b);
    }

    private void Victory()
    {
        SetGameActive(false);
        victoryScreen.SetActive(true);
        gameUI.SetActive(false);
        DataHolder.SetCoins(DataHolder.GetCoins() + DataHolder.GetCoinsPerRound());
        Time.timeScale = 0f;
    }

    private void PauseOrResume()
    {
        Time.timeScale = 1.0f - Time.timeScale;
        pauseText.SetActive(!pauseText.activeSelf);
    }

    private void SettingsMenu()
    {
    }

    void QuitGame()
    {
        Application.Quit();
    }

    void NextScene() { SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1); }

    public void EntityDied()
    {
        if (shopManager.revenge_mask_wielder !=  null)
            shopManager.revenge_mask_wielder.GetComponent<EntityBehavior>().UpdateRevengeMultiplier();
    }
}

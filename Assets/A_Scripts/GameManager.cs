using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private Button resume;
    [SerializeField] private Button quit;
    [SerializeField] private Button settings;
    [SerializeField] private TMP_Text round_text;
    [SerializeField] private TMP_Text time_text;
    [SerializeField] private TMP_Text coin_text;

    public List<GameObject> enemy_list;
    public List<GameObject> ally_list;
    public Dictionary<string, int> entity_max_occurences;

    public int coins;
    private float start_time;
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
        entity_max_occurences = new() { { "Warrior", 3 }, { "Tank", 3 } };
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
        UpdateTime();
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
                Debug.Log("Found Prefab: " + prefab.name);
            }
        }

        Debug.Log($"Total Prefabs Found: {prefabs.Count}");
        return prefabs.ToArray();
    }
}

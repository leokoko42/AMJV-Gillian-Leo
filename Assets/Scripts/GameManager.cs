using TMPro;
using Unity.VisualScripting;
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

    private float start_time;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        resume.onClick.AddListener(TogglePause);
        quit.onClick.AddListener(QuitGame);
        settings.onClick.AddListener(SettingsMenu);
        start_time = Time.time;
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
}

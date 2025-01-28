using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject base_menu;
    [SerializeField] private GameObject difficulty_menu;

    [SerializeField] private Button start;
    [SerializeField] private Button quit;
    [SerializeField] private Button easy_button;
    [SerializeField] private Button normal_button;
    [SerializeField] private Button hard_button;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.AddListener(ShowRulesMenu);
        quit.onClick.AddListener(QuitGame);
        easy_button.onClick.AddListener(() => SetDifficulty("Easy"));
        normal_button.onClick.AddListener(() => SetDifficulty("Normal"));
        hard_button.onClick.AddListener(() => SetDifficulty("Hard"));
    }

    // Update is called once per frame
    void Update()
    {

    }

    void ShowRulesMenu()
    {
        base_menu.SetActive(false);
        difficulty_menu.SetActive(true);
    }

    void SettingsMenu()
    {

    }

    void QuitGame()
    {
        Application.Quit();
    }
    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void SetDifficulty(string diff)
    {
        switch (diff)
        {
            case "Easy":
                DataHolder.SetCoinsPerRound(200);
                break;
            case "Normal":
                DataHolder.SetCoinsPerRound(150);
                break;
            case "Hard":
                DataHolder.SetCoinsPerRound(100);
                break;
            default:
                break;
        }
        StartGame();
    }
}

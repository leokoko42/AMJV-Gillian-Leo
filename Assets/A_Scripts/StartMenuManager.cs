using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenuManager : MonoBehaviour
{
    [SerializeField] private Button start;
    [SerializeField] private Button settings;
    [SerializeField] private Button quit;
    [SerializeField] private GameObject optionMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        start.onClick.AddListener(StartGame);
        quit.onClick.AddListener(QuitGame);
        settings.onClick.AddListener(SettingsMenu);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void StartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    void SettingsMenu()
    {

    }

    void QuitGame()
    {
        Application.Quit();
    }
}

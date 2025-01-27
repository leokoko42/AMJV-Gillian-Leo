using TMPro;
using Unity.VisualScripting;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TMP_Text coinsGained;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.onClick.AddListener(NextLevel);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void OnEnable()
    {
        coinsGained.text = "Coins Gained : " + gameManager.GetCoinsOnWin().ToString();
        gameObject.GetComponent<HideOnStart>().Unhide();
    }

    void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

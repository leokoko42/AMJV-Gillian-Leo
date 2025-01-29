using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] private Button nextButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private TMP_Text coinsGained;
    [SerializeField] private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nextButton.onClick.AddListener(NextLevel);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void OnEnable()
    {
        coinsGained.text = "Coins Gained : " + DataHolder.GetCoinsPerRound().ToString();
        gameObject.GetComponent<HideOnStart>().Unhide();
    }

    void NextLevel()
    {
        DataHolder.SetCoins(DataHolder.GetCoins() + DataHolder.GetCoinsPerRound());
        DataHolder.SetCoinsAtRoundStart(DataHolder.GetCoins());
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        Time.timeScale = 1.0f;
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DefeatScreen : MonoBehaviour
{
    [SerializeField] private Button retryButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameManager gameManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retryButton.onClick.AddListener(RetryLevel);
        quitButton.onClick.AddListener(QuitGame);
    }

    public void OnEnable()
    {
        TMP_Text retry_button_text = retryButton.GetComponentInChildren<TMP_Text>();
        if (DataHolder.GetRetries() == 0)
        {
            retryButton.enabled = false;
        }
        retry_button_text.text = "RETRY (" + DataHolder.GetRetries() + " Left)";
        gameObject.GetComponent<HideOnStart>().Unhide();
    }

    void RetryLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        DataHolder.SetCoins(DataHolder.GetCoinsAtRoundStart());
        DataHolder.SetRetries(DataHolder.GetRetries() - 1);
        gameObject.SetActive(false);
        gameManager.SetShopActive(true);
        Time.timeScale = 1.0f;
    }

    void QuitGame()
    {
        Application.Quit();
    }
}

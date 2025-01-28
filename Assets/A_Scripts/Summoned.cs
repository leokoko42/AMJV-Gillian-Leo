using UnityEngine;

public class Summoned : MonoBehaviour
{
    private AbilityManager summonerAbility;
    private GameManager gameManager;

    private void Start() {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameObject.tag == "Allies") {
            gameManager.AddAlly(gameObject);
        }
        else {
            gameManager.AddEnemy(gameObject);
        }
    }
    public void Init(AbilityManager summonerAbility) {
        this.summonerAbility = summonerAbility;
    }

    public void Death() {
        summonerAbility.RemoveSummonnedInstance(gameObject);
        if (gameObject.tag == "Allies") {
            gameManager.RemoveAlly(gameObject);
        }
        else {
            gameManager.RemoveEnemy(gameObject);
        }
    }
}

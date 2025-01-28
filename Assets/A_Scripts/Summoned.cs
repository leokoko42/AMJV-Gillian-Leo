using UnityEngine;

public class Summoned : MonoBehaviour
{
    private AbilityManager summonerAbility;

    public void Init(AbilityManager summonerAbility) {
        this.summonerAbility = summonerAbility;
    }

    public void Death() {
        summonerAbility.RemoveSummonnedInstance(gameObject);
    }
}

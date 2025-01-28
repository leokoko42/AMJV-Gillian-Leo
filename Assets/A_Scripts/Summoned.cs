using UnityEngine;

public class Summoned : MonoBehaviour
{
    private AbilityManager summonerAbility;
    

    void Update()
    {
        
    }

    public void Init(AbilityManager summonerAbility) {
        this.summonerAbility = summonerAbility;
    }
}

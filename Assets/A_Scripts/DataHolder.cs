using System.Data;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static DataHolder Instance;

    public int coins;
    private float start_time;
    private int retries;

    //Item stats
    public static float earring_reduction_factor;
    public static float belt_ability_mult;
    public static float halo_heal_precentage;
    public static float mask_revenge_boost;
    public static float vial_poison_precentage;
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        coins = 100;
        start_time = Time.time;

        earring_reduction_factor = 3f;
        belt_ability_mult = 1.25f;
        halo_heal_precentage = 0.2f;
        mask_revenge_boost = 0.02f;
        vial_poison_precentage = 0.1f;
    }

    public int GetCoins() { return coins; }
    public void SetCoins(int c) { coins = c; }

    public float GetStartTime() { return start_time; }

    public int GetRetries() { return retries; }

    public void SetRetries(int r) { retries = r; }

    public static float GetEarringReductionFactor() { return earring_reduction_factor; }
    public static float GetMaskRevengeBoost() { return mask_revenge_boost; }
    public static float GetHaloHealPercentage() { return halo_heal_precentage; }
    public static float GetBeltAbilityMult() { return belt_ability_mult; }
    public static float GetVialPoisonPercentage() { return vial_poison_precentage; }
    public static void SetEarringReductionFactor(float val) { earring_reduction_factor = val; }
    public static void SetMaskRevengeBoost(float val) { mask_revenge_boost = val; }
    public static void SetHaloHealPercentage(float val) { halo_heal_precentage = val; }
    public static void SetBeltAbilityMult(float val) { belt_ability_mult = val; }
    public static void SetVialPoisonPercentage(float val) { vial_poison_precentage = val; }
}

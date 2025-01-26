using System.Data;
using UnityEngine;

public class DataHolder : MonoBehaviour
{
    public static DataHolder Instance;

    public int coins;
    private float start_time;
    private int retries;

    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);

        coins = 100;
        start_time = Time.time;
    }

    public int GetCoins() { return coins; }
    public void SetCoins(int c) { coins = c; }

    public float GetStartTime() { return start_time; }

    public int GetRetries() { return retries; }

    public void SetRetries(int r) { retries = r; }
}

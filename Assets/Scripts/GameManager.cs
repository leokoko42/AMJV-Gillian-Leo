using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject ally;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private bool rPressed, hPressed;
    // Update is called once per frame
    void Update()
    {
        rPressed = Input.GetKeyDown(KeyCode.R);
    }

    void FixedUpdate() {
        if (rPressed) {
            summonTroop();
        }
    }

    void summonTroop() {
        Vector3 spawnPoint = new Vector3(0,0,0);
        Instantiate(ally);
    }
}

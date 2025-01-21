using UnityEngine;

public class Pits : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnTriggerEnter(Collider collider) {
        string tag = collider.gameObject.tag;
        if (tag == "Allies" || tag == "Enemies") {
            collider.gameObject.GetComponent<HealthManager>().Death();
            Debug.Log("KILL");
        }
    }
}

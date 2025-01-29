using UnityEngine;

public class DemoInactive : MonoBehaviour
{
    [SerializeField] BasicEntity basicEntity;
    void Update()
    {
        basicEntity.SetIsActive(false);
    }
}

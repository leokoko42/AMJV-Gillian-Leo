using UnityEngine;

public class UnitPlacement : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private GameObject warrior;
    private GameObject to_spawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        to_spawn = null;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerMask))
            {
                Vector3 point = hit.point;
                Vector3 spawn_pos = new Vector3(point.x, point.y + 1.5f, point.z);
                if (to_spawn != null)
                {
                    GameObject entity = Instantiate(to_spawn, spawn_pos, Quaternion.identity);
                    BasicEntity basicEntity = entity.GetComponent<BasicEntity>();
                    basicEntity.SetIsActive(false);
                    gameManager.AddAlly(entity);
                    Debug.Log(gameManager.ally_list);
                }
            }
        }
    }

    public void UnitToSpawn(string name)
    {
        switch (name)
        {
            case "Warrior":
                to_spawn = warrior;
                break;
            default:
                to_spawn = null;
                break;
        }
    }
}

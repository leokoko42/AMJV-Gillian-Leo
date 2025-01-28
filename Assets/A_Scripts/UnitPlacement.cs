using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.HID;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class UnitPlacement : MonoBehaviour
{
    [SerializeField] private DataHolder data;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private Camera _camera;
    [SerializeField] private Camera _UIcamera;
    [SerializeField] private LayerMask layerPlacement;
    [SerializeField] private LayerMask layerEntity;
    [SerializeField] private LayerMask layerUI;

    private GameObject to_spawn_object;
    private string to_spawn_name;
    private int spawn_cost;
    public Dictionary<string, int> entity_occurences;

    void Start()
    {
        to_spawn_object = null;
        GameObject[] ally_prefabs = gameManager.FindPrefabs("AllyEntities");
        entity_occurences = new Dictionary<string, int>();
        gameManager.entity_max_occurences = new Dictionary<string, int>();
        for (int i = 0; i < ally_prefabs.Length; i++)
        {
            entity_occurences.Add(ally_prefabs[i].name, 0);
            gameManager.entity_max_occurences.Add(ally_prefabs[i].name, 3);
            Debug.Log(ally_prefabs[i].name);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _UIcamera.ScreenPointToRay(mousePosition);
            if (!Physics.Raycast(ray, out _, float.MaxValue, layerUI))
            {
                if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerPlacement))
                {
                    Vector3 point = hit.point;
                    Vector3 spawn_pos = new Vector3(point.x, point.y + 1.5f, point.z);
                    if (to_spawn_object != null)
                    {
                        if (spawn_cost <= DataHolder.GetCoins() && entity_occurences[to_spawn_name] < gameManager.entity_max_occurences[to_spawn_name])
                        {
                            GameObject entity = Instantiate(to_spawn_object, spawn_pos, Quaternion.identity);
                            BasicEntity basicEntity = entity.GetComponent<BasicEntity>();
                            basicEntity.SetIsActive(false);
                            entity_occurences[to_spawn_name] += 1;
                            gameManager.RemoveCoins(basicEntity.GetCost());
                            gameManager.AddAlly(entity);
                        }
                    }
                }
            }
            
        }

        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            if (shopManager.GetSelectedEntity() != null)
            {
                if (!Physics.Raycast(ray, out _, float.MaxValue, layerUI))
                {
                    shopManager.DeselectEntity();
                }
            }
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerEntity))
            {
                if (hit.collider != null)
                {
                    GameObject entity = hit.transform.gameObject;
                    if (!entity.GetComponent<BasicEntity>().GetIsEnemy())
                    {
                        if (gameManager.GetShopActive()) 
                        { 
                            shopManager.SetSelectedEntity(entity);
                        }
                        
                    }
                }
            }
        }

        if (Input.GetMouseButtonDown(2))
        {
            Vector3 mousePosition = Input.mousePosition;
            Ray ray = _camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, layerEntity))
            {
                if (hit.collider != null)
                {
                    GameObject entity = hit.transform.gameObject;
                    gameManager.SetAllyKing(entity);
                }
            }
        }
    }

    public void UnitToSpawn(GameObject entity_obj)
    {
        to_spawn_name = entity_obj.name;
        to_spawn_object = entity_obj;
        spawn_cost = entity_obj.GetComponent<BasicEntity>().GetCost();
    }

    public void SellEntity(GameObject entity)
    {
        entity_occurences[entity.name.Replace("(Clone)", "")] -= 1;
        gameManager.AddCoins(entity.GetComponent<BasicEntity>().GetCost());
        gameManager.RemoveAlly(entity);
        entity.GetComponent<EntityBehavior>().SetItemEquipped("");
        Destroy(entity);
    }
}

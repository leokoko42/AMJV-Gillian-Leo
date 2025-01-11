using UnityEngine;

public class UIFollow : MonoBehaviour
{
    private GameObject _camera;
    void Start()
    {
        _camera = GameObject.FindGameObjectsWithTag("MainCamera")[0];
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        transform.forward = -_camera.transform.forward;
    }
}

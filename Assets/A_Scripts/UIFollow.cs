using UnityEngine;

public class UIFollow : MonoBehaviour
{
    [SerializeField] private bool scale;
    [SerializeField] private float scale_at_10u;
    [SerializeField] HideOnStart hideOnStart;
    private GameObject _camera;

    void Start()
    {
        _camera = GameObject.Find("UI Camera");
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.LookRotation(transform.position - _camera.transform.position);
        transform.forward = _camera.transform.forward;
        if (scale && hideOnStart.GetUnhidden())
        {
            float scale = Vector3.Distance(transform.position, _camera.transform.position) * scale_at_10u/10;
            transform.localScale = scale * Vector3.one;
        }
    }
}

using UnityEngine;

public class HideOnStart : MonoBehaviour
{
    private bool unhidden;
    private Vector3 original_scale;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        original_scale = transform.localScale;
        transform.localScale = Vector3.zero;
        unhidden = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Unhide()
    {
        if (!unhidden)
        {
            transform.localScale = original_scale;
            unhidden = true;
        }
    }

    public void Hide()
    {
        if (unhidden)
        {
            transform.localScale = Vector3.zero;
            unhidden = false;
        }
    }

    public bool GetUnhidden()
    {
        return unhidden;
    }

    public void SetUnhidden(bool b)
    {
        unhidden = b;
    }
}

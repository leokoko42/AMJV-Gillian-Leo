using UnityEngine;

public class UI_Bar : MonoBehaviour
{
    private float width;
    private float height;
    private RectTransform rectTransform;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        width = rectTransform.rect.width;
        height = rectTransform.rect.height;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateBar(float new_health, float max_health)
    {
        if (transform.localScale != Vector3.one)
        {
            transform.localScale = Vector3.one;
        }
        RectTransform rect = GetComponent<RectTransform>();
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width * new_health/max_health);
        rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
    }
}

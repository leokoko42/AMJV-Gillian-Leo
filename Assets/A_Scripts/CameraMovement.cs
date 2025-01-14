using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private float movement_speed;
    [SerializeField] private float rotation_speed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.forward;
        Vector3 better_forward = new Vector3(forward.x, 0, forward.z).normalized;
        transform.position += (new Vector3(0, 1, 0) * Input.GetAxis("Shift-Space") + transform.right * Input.GetAxis("WASD-Horizontal") + better_forward * Input.GetAxis("WASD-Vertical"))  * movement_speed * Time.deltaTime;
        transform.Rotate(new Vector3(-1, 0, 0) * Input.GetAxis("Vertical") * rotation_speed * Time.deltaTime, Space.Self);
        transform.Rotate(new Vector3(0, 1, 0) * Input.GetAxis("Horizontal") * rotation_speed * Time.deltaTime, Space.World);
    }

    private void FixedUpdate()
    {
    }

    /*private void Movement(Vector3 dir, float speed)
    {
        float lerpSpeed = speed > rigi.linearVelocity.magnitude ? accel : decel;
        rigi.linearVelocity = Vector3.Lerp(rigi.linearVelocity, dir * speed, lerpSpeed);

    }*/
}

using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BulletMovment : MonoBehaviour
{
    float bulletSpeed;
    [SerializeField] GameObject target;
    float shooterPos, targetPos, targetVelocity;
    private float g, y_0, x, z;
    public float y_Max;
    private Rigidbody bulletRigidbody;
    private bool lauch, autoAim;
    
    public void Update() {
        Debug.Log(Vector3.Magnitude(bulletRigidbody.linearVelocity));
        if (autoAim) {
            AutoAim();
        }
        if (Input.GetMouseButtonDown(0))
        {
            transform.position = new Vector3(0, 2, 0);
            lauch = true;
            autoAim = false;
            LaunchBall();
        }
        if (lauch) {
            if (bulletRigidbody.linearVelocity.y<0) {
                autoAim = true;
            }
            transform.LookAt(transform.position + bulletRigidbody.linearVelocity);
        }
    }

    void Start()
    {
        autoAim = false;
        lauch = false;
        bulletRigidbody = GetComponent<Rigidbody>();
        g = 9.8f;
    }


    void LaunchBall()
    {
        Vector3 delta = target.transform.position - transform.position;
        float distance = Vector3.Magnitude(Vector3.ProjectOnPlane(delta, Vector3.up));
        float randomDelta = UnityEngine.Random.value * 0.5f;
        //Debug.Log(y_Max);
        float tetha = (20f + randomDelta)* distance / (5 + distance);
        y_Max = distance / 2 * Mathf.Tan(tetha * Mathf.Deg2Rad);

        y_0 = Math.Max(transform.position.y,target.transform.position.y);
        x = target.transform.position.x - transform.position.x;
        z = target.transform.position.z - transform.position.z;

        bulletRigidbody.useGravity = true;
    
        Vector3 displacementXZ = new Vector3(x, 0, z);

        // Implement equations derived from kinematic analysis
        Vector3 velocityY = Vector3.up*Mathf.Sqrt(2*g*(y_Max));
        Vector3 velocityXZ = displacementXZ/(Mathf.Sqrt(2*(y_Max)/g) + Mathf.Sqrt(2*y_Max/g));
        
        Vector3 velocity = velocityXZ + velocityY;
        bulletSpeed = Vector3.Magnitude(velocity);
        bulletRigidbody.linearVelocity = velocity*1.1f;
    }

    private void AutoAim() {
        bulletRigidbody.useGravity = false;
        transform.LookAt(target.transform);
        Vector3 bulletVelocity = bulletRigidbody.linearVelocity; 
        float speed = Vector3.Magnitude(bulletVelocity);
        float t = (1 + speed) / (15 + speed);
        bulletRigidbody.linearVelocity = Vector3.Lerp(bulletVelocity, transform.forward * (bulletSpeed + speed/10), t);
    }
}

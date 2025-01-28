using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions.Must;

public class BulletMovment : MonoBehaviour
{
    [SerializeField] private bool isAimed;
    private GameObject target, summoner;
    private float g, y_0, x, z, bulletSpeed, attack;
    private Rigidbody bulletRigidbody;
    private bool lauch, autoAim;
    [SerializeField] private float theta = 20f;
    
    public void Init(float new_attack,GameObject new_summoner) {
        summoner = new_summoner;
        attack = new_attack;
    }
    public void Init(float new_attack, GameObject new_target, GameObject new_summoner) {
        target = new_target;
        summoner = new_summoner;
        attack = new_attack;
    }
    public void FixedUpdate() {
        if (autoAim) {
            AutoAim();
        }
        if (isAimed) {
            if (bulletRigidbody.linearVelocity.y<0) {
                autoAim = true;
            }
            transform.LookAt(transform.position + bulletRigidbody.linearVelocity);
        }
        else {
            transform.eulerAngles = new Vector3(90,0,0);
        }
    }

    void Start()
    {
        autoAim = false;
        bulletRigidbody = GetComponent<Rigidbody>();
        g = 9.8f;
        if(isAimed) {
            LaunchBullet();
        }
    }

    public void LaunchBullet()
    {
        autoAim = false;
        
        Vector3 delta = target.transform.position - transform.position;
        float distance = Vector3.Magnitude(Vector3.ProjectOnPlane(delta, Vector3.up));
        float randomDelta = UnityEngine.Random.value * 0.5f;
        float new_theta = (theta + randomDelta)* distance / (5 + distance);
        float y_Max = distance / 2 * Mathf.Tan(new_theta * Mathf.Deg2Rad);

        y_0 = Math.Max(transform.position.y,target.transform.position.y);
        x = target.transform.position.x - transform.position.x;
        z = target.transform.position.z - transform.position.z;

        bulletRigidbody.useGravity = true;

        Vector3 displacementXZ = new Vector3(x, 0, z);

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

    public void OnTriggerEnter(Collider collider) {
        string colliderTag = collider.gameObject.tag;
        if (isAimed) {
            if (collider.gameObject == target) {
                HealthManager targetHealth = target.GetComponent<HealthManager>();
                targetHealth.Damage(attack);
                Destroy(gameObject);
            }
        }
        else {
            if (summoner.tag != colliderTag && (colliderTag == "Allies" || colliderTag == "Enemies")) {
                HealthManager targetHealth = collider.GetComponent<HealthManager>();
                targetHealth.Damage(attack);
                Destroy(gameObject);
            }
        }
        if (colliderTag == "Obstacle" || colliderTag == "Ground") {
            Destroy(gameObject);
        }
    }
}

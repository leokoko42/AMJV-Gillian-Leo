using Unity.VisualScripting;
using UnityEngine;

public class ChatGPT : MonoBehaviour
{
    public GameObject targetObject;
    private Transform target; // Target object to land on
    public Transform projectile; // Object to throw
    public float initialSpeed; // Initial speed of the projectile

    // Gravity (default is Physics.gravity.y)
    private float gravity;
    private bool isFlying;

    void Start()
    {
        target = targetObject.GetComponent<Transform>();
        // Use Unity's physics gravity
        gravity = Mathf.Abs(Physics.gravity.y);
        isFlying = false;
    }

    public void Update() {
        if (Input.GetMouseButton(0)) {
            isFlying = true;
            ThrowProjectile();
        }
        if (isFlying) {
            transform.LookAt(target);
        }
    }

    public void OnCollisionEnter(Collision collision) {
        if (collision.gameObject == targetObject) {
            isFlying = false;
            Destroy(gameObject);
        } 
    }
    public void ThrowProjectile()
    {
        if (projectile == null || target == null)
        {
            Debug.LogError("Projectile or target is not assigned.");
            return;
        }

        // Calculate the velocity needed to hit the target
        Vector3 velocity = CalculateVelocity(target.position, projectile.position, initialSpeed);

        if (velocity != Vector3.zero)
        {
            Rigidbody rb = projectile.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = velocity;
            }
            else
            {
                Debug.LogError("Projectile requires a Rigidbody component.");
            }
        }
    }

    private Vector3 CalculateVelocity(Vector3 targetPosition, Vector3 startPosition, float speed)
    {
        Vector3 direction = targetPosition - startPosition; // Direction to the target
        Vector3 directionXZ = new Vector3(direction.x, 0, direction.z); // Ignore vertical for horizontal distance

        float distance = directionXZ.magnitude; // Horizontal distance
        float heightDifference = direction.y; // Vertical distance

        float speedSquared = speed * speed;
        float underSquareRoot = (speedSquared * speedSquared) - gravity * (gravity * distance * distance + 2 * heightDifference * speedSquared);

        if (underSquareRoot < 0)
        {
            Debug.LogError("No valid trajectory. Increase initial speed or adjust positions.");
            return Vector3.zero; // No valid solution
        }

        float root = Mathf.Sqrt(underSquareRoot);
        float angleUp = Mathf.Atan2(speedSquared + root, gravity * distance);
        float angleDown = Mathf.Atan2(speedSquared - root, gravity * distance);

        // Use the smaller angle for a more direct trajectory
        float angle = angleDown < angleUp ? angleDown : angleUp;

        Vector3 velocity = directionXZ.normalized * Mathf.Cos(angle) * speed;
        velocity.y = Mathf.Sin(angle) * speed;

        return velocity;
    }

    void OnDrawGizmos()
    {
        if (projectile != null && target != null)
        {
            // Draw a line to visualize the trajectory
            Gizmos.color = Color.red;
            Vector3 start = projectile.position;
            Vector3 velocity = CalculateVelocity(target.position, start, initialSpeed);

            if (velocity != Vector3.zero)
            {
                Vector3 previous = start;
                for (float t = 0; t < 5; t += 0.1f)
                {
                    Vector3 next = previous + velocity * 0.1f + 0.5f * Physics.gravity * Mathf.Pow(0.1f, 2);
                    Gizmos.DrawLine(previous, next);
                    velocity += Physics.gravity * 0.1f;
                    previous = next;
                }
            }
        }
    }
}

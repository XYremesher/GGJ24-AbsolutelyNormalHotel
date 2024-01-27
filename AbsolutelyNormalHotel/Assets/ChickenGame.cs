using UnityEngine;

public class ChickenGame : MonoBehaviour
{
    public float speed = 1;
    public float initialSpeed = 0.5f;
    public float speedIncreaseRate = 0.1f;
    public float jumpForce = 5;
    public float glideForce = 0.5f;
    private Rigidbody rb;

    public float groundCheckDistance = 0.5f;

    private float jumpTimer;
    private float nextJumpTime;

    private bool isGrounded;
    private bool isFirstGrounded = true;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ResetJumpTimer();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    void Update()
    {
        isGrounded = IsGrounded();
        float currentSpeed = isFirstGrounded ? initialSpeed : speed;
        transform.localPosition += transform.forward * currentSpeed * Time.deltaTime;

        if (isFirstGrounded && currentSpeed < speed)
        {
            initialSpeed += speedIncreaseRate * Time.deltaTime;
        }

        jumpTimer -= Time.deltaTime;
        if (jumpTimer <= 0 && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isFirstGrounded = false;
            ResetJumpTimer();
        }

        ApplyAirForces();
    }

    private void ResetJumpTimer()
    {
        nextJumpTime = Random.Range(1f, 2f);
        jumpTimer = nextJumpTime;
    }

    private void ApplyAirForces()
    {
        if (!isGrounded)
        {
            // When ascending
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.up * -glideForce);
            }
            // When descending
            else if (rb.velocity.y < 0)
            {
                EaseDescent();
            }
        }
    }

    private void EaseDescent()
    {
        // Calculate the easing factor based on the chicken's altitude or other criteria
        float easeFactor = CalculateEaseFactor();

        // Apply the ease factor to gradually reduce the vertical velocity
        Vector3 newVelocity = rb.velocity;
        newVelocity.y *= easeFactor;
        rb.velocity = newVelocity;
    }


    private float CalculateEaseFactor()
    {
        float speed = Mathf.Abs(rb.velocity.y);
        float maxSpeed = jumpForce;
        return 1 - Mathf.Clamp01(speed / maxSpeed);
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        return Physics.Raycast(transform.position, Vector3.down, out hit, groundCheckDistance)
            && hit.collider.CompareTag("Ground");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Ground")
        {
            Vector3 hitNormal = collision.contacts[0].normal;
            hitNormal.y = 0; // Ignore vertical component of the normal

            // Calculate the new direction on the horizontal plane
            Vector3 newDirection = Vector3.Reflect(transform.forward, hitNormal);

            // Reset angular velocity to prevent the chicken from tilting or rolling
            rb.angularVelocity = Vector3.zero;

            // Calculate the new Y rotation while keeping X and Z rotations as zero
            float newYRotation = Quaternion.LookRotation(newDirection).eulerAngles.y;
            transform.rotation = Quaternion.Euler(0, newYRotation, 0);
        }
    }


    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 5f;

    private Rigidbody rb;
    private Animator anim;
    private bool isJumping = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // Movement input
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized;

        // Running with Shift
        float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : moveSpeed;
        rb.velocity = new Vector3(movement.x * speed, rb.velocity.y, movement.z * speed);

        // Set Animator Parameters
        anim.SetFloat("Speed", movement.magnitude * speed);

        // Jumping
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            anim.SetBool("IsJumping", true);
        }

        // Fix character rotation
        if (movement.magnitude > 0)
        {
            transform.rotation = Quaternion.LookRotation(movement);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            anim.SetBool("IsJumping", false);
        }
    }
}

using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float jogSpeed = 4f;
    public float jumpForce = 6f; // Reduced jump height
    public float skateSpeed = 6f;
    private float currentSpeed;

    private Rigidbody rb;
    private Animator animator;
    private bool isJumping = false;
    private bool isSkateboarding = false;
    private bool isPushing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = walkSpeed; // Start with walking speed
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
        HandleSkateboarding();
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * currentSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Rotate character to movement direction
        if (movement.magnitude > 0)
        {
            transform.forward = movement;
        }

        // Handle Animations
        bool isMoving = movement.magnitude > 0;

        animator.SetBool("isIdle", !isMoving && !isSkateboarding);
        animator.SetBool("isWalking", isMoving && !Input.GetKey(KeyCode.LeftShift) && !isSkateboarding);
        animator.SetBool("isJogging", isMoving && Input.GetKey(KeyCode.LeftShift) && !isSkateboarding);
        animator.SetBool("isSkateboarding", isSkateboarding && !isJumping);
        animator.SetBool("isPushing", isPushing && !isJumping);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            // Only jumping animation should play
            animator.SetBool("isJumping", true);
            animator.SetBool("isSkateboarding", false);
            animator.SetBool("isPushing", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isJogging", false);
        }
    }

    void HandleSkateboarding()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Toggle skateboarding mode
        {
            isSkateboarding = !isSkateboarding;

            if (isSkateboarding)
            {
                currentSpeed = skateSpeed;
                animator.SetBool("isSkateboarding", true);
                animator.SetBool("isIdle", false);
            }
            else
            {
                currentSpeed = walkSpeed;
                animator.SetBool("isSkateboarding", false);
                animator.SetBool("isPushing", false);
                animator.SetBool("isIdle", true); // Reset to idle animation
            }
        }

        if (isSkateboarding)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isPushing = true;
                animator.SetBool("isPushing", true);
                animator.SetBool("isJogging", false); // Prevent jogging animation
                currentSpeed = skateSpeed * 2f; // Increase speed while pushing
            }
            else
            {
                isPushing = false;
                animator.SetBool("isPushing", false);
                currentSpeed = skateSpeed;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
            animator.SetBool("isJumping", false);

            if (isSkateboarding)
            {
                animator.SetBool("isSkateboarding", true);
            }
        }
    }
}

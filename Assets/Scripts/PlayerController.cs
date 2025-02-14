using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 2f;
    public float jogSpeed = 6f;
    public float jumpForce = 6f; 
    public float skateSpeed = 6f;
    private float currentSpeed;

    private Rigidbody rb;
    private Animator animator;
    private bool isJumping = false;
    private bool isSkateboarding = false;
    private bool isPushing = false;

    public GameObject skateboardPrefab; 
    private GameObject skateboardInstance;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        currentSpeed = walkSpeed; 

        // Skateboard spawn (but is initially disabled)
        if (skateboardPrefab != null)
        {
            skateboardInstance = Instantiate(skateboardPrefab, transform.position, Quaternion.identity);
            skateboardInstance.SetActive(false); // Hide skateboard at start
        }
    }

    void Update()
    {
        MovePlayer();
        HandleJump();
        HandleSkateboarding();

        if (isSkateboarding && skateboardInstance != null)
        {
            UpdateSkateboardPosition(); // Make skateboard follow player
        }
    }

    void MovePlayer()
    {
        float moveX = Input.GetAxis("Horizontal"); // A/D or Left/Right
        float moveZ = Input.GetAxis("Vertical");   // W/S or Up/Down

        if (!isSkateboarding)
        {
            currentSpeed = Input.GetKey(KeyCode.LeftShift) ? jogSpeed : walkSpeed;
        }

        Vector3 movement = new Vector3(moveX, 0, moveZ).normalized * currentSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);

        // Rotate character to movement direction
        if (movement.magnitude > 0)
        {
            transform.forward = movement;
        }

        // Handle Animations
        animator.SetBool("isIdle", movement.magnitude == 0 && !isSkateboarding);
        animator.SetBool("isWalking", movement.magnitude > 0 && !Input.GetKey(KeyCode.LeftShift) && !isSkateboarding);
        animator.SetBool("isJogging", movement.magnitude > 0 && Input.GetKey(KeyCode.LeftShift) && !isSkateboarding);
        animator.SetBool("isSkateboarding", isSkateboarding && !isJumping);
        animator.SetBool("isPushing", isPushing && !isJumping);
    }

    void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;

            animator.SetBool("isJumping", true);
            animator.SetBool("isSkateboarding", false);
            animator.SetBool("isPushing", false);
        }
    }

    void HandleSkateboarding()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl)) // Toggle skateboarding mode
        {
            isSkateboarding = !isSkateboarding; // Flip state

            if (isSkateboarding)
            {
                currentSpeed = skateSpeed;
                animator.SetBool("isSkateboarding", true);

                if (skateboardInstance != null)
                {
                    skateboardInstance.SetActive(true); // Show skateboard
                }
            }
            else
            {
                currentSpeed = walkSpeed;
                animator.SetBool("isSkateboarding", false);
                animator.SetBool("isPushing", false);
                animator.SetBool("isIdle", true); // Reset to idle animation

                if (skateboardInstance != null)
                {
                    skateboardInstance.SetActive(false); // Hide skateboard
                }
            }
        }

        if (isSkateboarding)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isPushing = true;
                animator.SetBool("isPushing", true);
                currentSpeed = skateSpeed * 2f; 
            }
            else
            {
                isPushing = false;
                animator.SetBool("isPushing", false);
                currentSpeed = skateSpeed;
            }
        }
    }

    void UpdateSkateboardPosition()
    {
        if (skateboardInstance != null)
        {
            Vector3 newPos = transform.position;
            newPos.y = 0.35f; 
            skateboardInstance.transform.position = newPos;

            // Rotate skateboard to match the player movement direction
            skateboardInstance.transform.rotation = Quaternion.LookRotation(transform.forward);
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundCheckRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private bool isWalkingOnCeiling = false;
    private Camera mainCamera;
    private float playerWidth;  // Assuming the player sprite has a box collider
    private bool canToggleShift = true; // Added variable for cooldown
    public float shiftToggleCooldown = 1.0f; // Adjust the cooldown time as needed

    private bool canJump = true;  // Added flag to control jumping
    private bool isAlive = true;  // Added flag to track player's life

    private static int currentLevel = 1; // Track the current level

    // Reference to the timer script
    private Timer timer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCamera = Camera.main;

        // Get the width of the player sprite (assuming it has a BoxCollider2D)
        playerWidth = GetComponent<BoxCollider2D>().bounds.extents.x;

        // Find and store a reference to the Timer script
        timer = FindObjectOfType<Timer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAlive)
        {
            float move = Input.GetAxis("Horizontal");
            // Adjust sprite orientation based on movement direction
            if ((move > 0 && !isWalkingOnCeiling) || (move < 0 && isWalkingOnCeiling))
            {
                // Facing right when walking on the ground or left when walking on the ceiling
                transform.localScale = new Vector3(1, 1, 1);
            }
            else if ((move < 0 && !isWalkingOnCeiling) || (move > 0 && isWalkingOnCeiling))
            {
                // Facing left when walking on the ground or right when walking on the ceiling
                transform.localScale = new Vector3(-1, 1, 1);
            }

            rb.velocity = new Vector2(move * speed, rb.velocity.y);

            bool isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

            // Optional: Set animator parameters for animations based on isGrounded.

            if (isGrounded && Input.GetButtonDown("Jump") && canJump)
            {
                PerformJump(jumpForce);
                canJump = false;  // Disable jumping until the player lands
            }
            else if (isWalkingOnCeiling && Input.GetButtonDown("Jump") && canJump)
            {
                PerformJump(-jumpForce); // Jump with opposite force to go up while on the ceiling
                canJump = false;  // Disable jumping until the player lands
            }

            // Toggle walking on ceiling when Shift is pressed
            if (Input.GetKeyDown(KeyCode.LeftShift) && canToggleShift)
            {
                shiftGravity();

                // Start cooldown
                canToggleShift = false;
                Invoke("ResetShiftToggleCooldown", shiftToggleCooldown);
            }

            // Clamp player position to camera bounds
            ClampPlayerToCameraBounds();
        }
    }

    void ClampPlayerToCameraBounds()
    {
        Vector3 playerPos = transform.position;
        Vector3 viewPos = mainCamera.WorldToViewportPoint(playerPos);

        float minX = mainCamera.ViewportToWorldPoint(new Vector3(0, 0, viewPos.z)).x + playerWidth;
        float maxX = mainCamera.ViewportToWorldPoint(new Vector3(1, 0, viewPos.z)).x - playerWidth;

        playerPos.x = Mathf.Clamp(playerPos.x, minX, maxX);

        transform.position = playerPos;
    }

    void PerformJump(float jumpForce)
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player has landed on the ground or ceiling
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Ceiling"))
        {
            canJump = true;  // Enable jumping upon landing
        }
    }

    void shiftGravity()
    {
        isWalkingOnCeiling = !isWalkingOnCeiling;

        // Rotate the player 180 degrees to simulate walking on the ceiling
        // Change player sprite to "dead.png"
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            if (isWalkingOnCeiling) spriteRenderer.flipX = true;
            else spriteRenderer.flipX = false;
            transform.Rotate(Vector3.forward, 180f);
        }

        // Reverse the gravity to simulate walking on the ceiling
        rb.gravityScale *= -1f;
    }

    void ResetShiftToggleCooldown()
    {
        canToggleShift = true; // Reset cooldown
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (isAlive)
        {
            // Check for collision with obstacle
            if (collision.gameObject.CompareTag("Obstacle"))
            {
                if (isWalkingOnCeiling) shiftGravity();

                Die();
            }
        }
    }

    void Die()
    {
        isAlive = false;
        // Change player sprite to "dead.png"
        // Stop player movement or add any other game over logic

        // Stop the Animator component
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Change player sprite to "dead.png"
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            // Load and assign the "dead.png" sprite
            Sprite deadSprite = Resources.Load<Sprite>("dead");
            if (deadSprite != null)
            {
                spriteRenderer.sprite = deadSprite;
                
                transform.rotation = Quaternion.Euler(0f, 0f, -90f);
            }
            else
            {
                Debug.LogError("Failed to load 'dead.png' sprite!");
            }
        }
        else
        {
            Debug.LogError("SpriteRenderer component not found on the player GameObject!");
        }

        // Call StopSpawning on the obstacle spawner
        FindObjectOfType<ObstacleSpawner>().StopSpawning();

        // Stop the timer
        if (timer != null)
        {
            timer.StopTimer();
        }
    }

    public void Win()
    {
        isAlive = false;
        // Stop player movement or add any other game over logic

        if (isWalkingOnCeiling) shiftGravity();

        // Stop the Animator component
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.enabled = false;
        }

        // Call StopSpawning on the obstacle spawner
        FindObjectOfType<ObstacleSpawner>().StopSpawning();

        // Stop the timer
        if (timer != null)
        {
            timer.StopTimer();
        }

        // Load the next level after a delay (optional)
        float delayBeforeNextLevel = 3f; // Adjust the delay as needed
        Invoke("LoadNextLevel", delayBeforeNextLevel);
    }

    void LoadNextLevel()
    {
        currentLevel++;
        if (currentLevel < 5) SceneManager.LoadScene("Level" + currentLevel);
    }
}

using UnityEngine.UI;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private float speed = 5f;

    void Start()
    {
        Timer timer = FindAnyObjectByType<Timer>();
        if (timer != null )
        {
            speed = timer.obstacleSpeed;
        }
        // Give the obstacle a leftward velocity
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(-speed, 0);
    }

    void Update()
    {
        // Check if the obstacle is off-screen and destroy it
        if (!GetComponent<Renderer>().isVisible)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Player collided with obstacle, implement game over logic here
            Debug.Log("Game Over!");

            // Find the Text component on the Canvas
            Text messageText = GameObject.Find("User Interface").GetComponent<Text>();

            if (messageText != null)
            {
                // Set the message text
                messageText.text = "You couldn't escape.";
            }
            else
            {
                Debug.LogError("Text component on Canvas not found!");
            }
        }
    }
}

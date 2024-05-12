using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    public Text timerText;
    private float startTime;
    private bool isRunning = true;

    public float obstacleSpeed = 5f;
    private float lastLoggedSecond = -1;

    void Start()
    {
        startTime = Time.time;
    }

    void Update()
    {
        if (isRunning)
        {
            float elapsedTime = Time.time - startTime;

            // Convert seconds to minutes and seconds
            float minutes = Mathf.FloorToInt(elapsedTime / 60);
            float seconds = Mathf.FloorToInt(elapsedTime % 60);

            // Increase obstacle speed every 5 seconds
            if (seconds > 0 && seconds % 5 == 0 && seconds != lastLoggedSecond)
            {
                obstacleSpeed += 1;
                Debug.Log("Speed increased to " + obstacleSpeed);
                lastLoggedSecond = seconds;
            }

            // Update the timer text
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (timerText.text.Equals("01:00"))
            {
                // Find the Text component on the Canvas
                Text messageText = GameObject.Find("User Interface").GetComponent<Text>();

                if (messageText != null)
                {
                    messageText.text = "You escaped!";

                    // Stop player movement here (you need to implement FreezeMovement in your PlayerController script)
                    // Call Win on the Player Controller
                    FindObjectOfType<PlayerController>().Win();
                }
                else
                {
                    Debug.LogError("Text component on Canvas not found!");
                }
                
            }
        }
    }

    // Method to stop the timer
    public void StopTimer()
    {
        isRunning = false;
    }
}
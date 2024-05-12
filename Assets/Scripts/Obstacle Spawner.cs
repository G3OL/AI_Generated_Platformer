using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject groundObstaclePrefab;
    public GameObject ceilingObstaclePrefab;
    public GameObject middleObstaclePrefab;

    float spawnerYPosition;
    private float offset;
    // Adjust the spawn position based on the size of the obstacle
    float obstacleHeight = 0f;

    private bool spawnEnabled = true;  // Added flag to control obstacle spawning

    void Start()
    {
        // Spawn obstacles after 3 seconds and then every 2 to 4 seconds
        InvokeRepeating("SpawnObstacle", 3f, Random.Range(2f, 4f));

        // Assuming there's a reference to the PlayerSpawner script
        PlayerSpawner playerSpawner = FindObjectOfType<PlayerSpawner>();

        if (playerSpawner != null)
        {
            spawnerYPosition = playerSpawner.GetSpawnerYPosition();
        }
        else
        {
            Debug.LogError("PlayerSpawner script not found in the scene.");
        }
    }

    void SpawnObstacle()
    {
        if (spawnEnabled)
        {
            offset = 15 * spawnerYPosition;

            // Randomly select obstacle type
            int obstacleType = Random.Range(0, 3);

            GameObject obstaclePrefab = null;
            Vector3 spawnPosition = Vector3.zero; // Default spawn position is the origin
            spawnPosition.x = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0, 0)).x;

            // Select the corresponding prefab based on the obstacle type
            switch (obstacleType)
            {
                case 0:
                    obstaclePrefab = groundObstaclePrefab;
                    obstacleHeight = groundObstaclePrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
                    spawnPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(0, Mathf.Clamp01((spawnerYPosition - 0) / (1 - 0)) + obstacleHeight / 5, 0)).y; // Ground obstacle at the bottom
                    break;
                case 1:
                    obstaclePrefab = ceilingObstaclePrefab;
                    // Ceiling obstacle at the right side of the screen
                    // Adjust the obstacleHeight based on the size of the ceiling obstacle
                    obstacleHeight = ceilingObstaclePrefab.GetComponent<SpriteRenderer>().bounds.extents.y;
                    spawnPosition.y = Camera.main.ViewportToWorldPoint(new Vector3(0, 1 - Mathf.Clamp01((spawnerYPosition - 0) / (1 - 0)) - obstacleHeight / 5, 0)).y; // Ceiling obstacle at the top
                    break;
                case 2:
                    obstaclePrefab = middleObstaclePrefab;
                    break;
            }

            // Offset the spawn position based on the camera's current position
            spawnPosition.x += Camera.main.transform.position.x;
            
            // Spawn the selected obstacle
            Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
            
        }
    }

    // Call this method to stop obstacle spawning
    public void StopSpawning()
    {
        spawnEnabled = false;
        CancelInvoke("SpawnObstacle");  // Cancel the spawning InvokeRepeating
    }
}
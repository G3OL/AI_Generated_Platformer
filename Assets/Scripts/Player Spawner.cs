using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        Instantiate(playerPrefab, transform.position, Quaternion.identity);
    }

    // Expose the y-position through a property
    public float GetSpawnerYPosition()
    {
        return transform.position.y;
    }
}

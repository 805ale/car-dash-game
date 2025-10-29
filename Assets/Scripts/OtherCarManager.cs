using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Manages traffic cars on the road using an object pooling approach.
/// It spawns inactive cars ahead, reuses them when they go behind the player,
/// and randomizes their direction and lane placement.
/// </summary>
public class OtherCarManager : MonoBehaviour
{
    // --- Parent references for organization in the Hierarchy ---
    
    [Header("Parent Containers")]
    [SerializeField] private GameObject activeCars;   // Holds currently active (visible) traffic cars
    [SerializeField] private GameObject unactiveCars; // Holds pooled/inactive cars ready for reuse

    // --- Pool setup ---
    [Header("Car Pool Settings")]
    [SerializeField] private GameObject[] carArray;   // All possible car prefabs to use
    [SerializeField] private int initialCarNumbers = 50; // How many cars to pre-instantiate in the pool

    // --- Spawn probabilities ---
    [Header("Spawn Frequency Settings")]
    [Range(0f, 1f)] public float carFreq;         // Probability to spawn a car on a given path segment
    [Range(0f, 1f)] public float reverseCarFreq;  // Probability that a spawned car drives the opposite direction

    // Keeps track of the last spawned car position to avoid placing two cars on the same lane consecutively
    public Vector3 previousCarPos = Vector3.zero;

    // Reference to the PathManager that holds all path segments (needed to access lanes/positions)
    [SerializeField] private PathManager pathManager;

    // -------------------------------------------------------------

    private void Start()
    {
        // Pre-fill the object pool with inactive cars at startup
        // This avoids performance spikes during gameplay from repeated Instantiate calls.
        for (int i = 0; i < initialCarNumbers; i++)
        {
            // Pick a random car prefab
            GameObject car = Instantiate(carArray[UnityEngine.Random.Range(0, carArray.Length)]);
            
            // Deactivate it so it doesn't render or simulate yet
            car.SetActive(false);

            // Store it under the "unactiveCars" container for later use
            car.transform.parent = unactiveCars.transform;
        }
    }

    // -------------------------------------------------------------
    /// <summary>
    /// Builds a list of potential spawn positions for cars based on the given path segment.
    /// Each child object of the path acts as a lane marker or position reference.
    /// </summary>
    List<Vector3> ListCarPosition(GameObject thePath)
    {
        List<Vector3> listPathPosition = new List<Vector3>();

        // Iterate through all lane markers (children) of this path prefab
        for (int i = 0; i < thePath.transform.childCount; i++)
        {
            // Add a small vertical offset so cars don’t clip into the road mesh
            Vector3 pos = thePath.transform.GetChild(i).position + new Vector3(0, 1, 0);
            listPathPosition.Add(pos);
        }

        return listPathPosition;
    }

    // -------------------------------------------------------------
    /// <summary>
    /// Retrieves a random car from the inactive pool, activates it,
    /// and detaches it from the unactiveCars container so it can be positioned in the scene.
    /// </summary>
    GameObject GetRandomCar()
    {
        // Randomly select one inactive car
        GameObject car = unactiveCars.transform.GetChild(
            UnityEngine.Random.Range(0, unactiveCars.transform.childCount)
        ).gameObject;

        // Detach from the pool parent
        car.transform.parent = null;

        // Activate the car (visible + simulated)
        car.SetActive(true);

        return car;
    }

    // -------------------------------------------------------------
    /// <summary>
    /// Called when a new path segment is spawned to possibly place a new traffic car on it.
    /// The chance of spawning is controlled by `carFreq`.
    /// </summary>
    public void CheckAndDisableCarPath()
    {
        // Get all potential car positions from the most recently recycled path
        List<Vector3> listCarPos = ListCarPosition(pathManager.pathList[pathManager.listPathIndex]);

        // Random chance check for whether a car should spawn on this segment
        if (UnityEngine.Random.value <= carFreq)
        {
            // Pick a random spawn position
            Vector3 carPos = listCarPos[UnityEngine.Random.Range(0, listCarPos.Count)];

            // Ensure we don't spawn two cars in the exact same lane back-to-back
            while (carPos.x == previousCarPos.x)
            {
                carPos = listCarPos[UnityEngine.Random.Range(0, listCarPos.Count)];
            }

            // Remember this position for the next spawn
            previousCarPos = carPos;

            // Pull a car from the inactive pool
            GameObject car = GetRandomCar();

            // Place the car slightly above the road
            car.transform.position = carPos;

            // Randomly decide if this car should face backward (opposite traffic)
            if (UnityEngine.Random.value <= reverseCarFreq)
            {
                car.transform.rotation = Quaternion.Euler(0f, 180f, 0f);
            }

            // Parent it to the active container for scene organization
            car.transform.parent = activeCars.transform;
        }
    }

    // -------------------------------------------------------------
    /// <summary>
    /// Checks all active cars and returns those that have gone behind
    /// the "destroyDistance" threshold to the inactive pool for reuse.
    /// </summary>
    public void FindCarAndReset()
    {
        for (int i = 0; i < activeCars.transform.childCount; i++)
        {
            // If a car is far behind the player/camera, recycle it
            if (activeCars.transform.GetChild(i).position.z < pathManager.destroyDistance)
            {
                GameObject theCar = activeCars.transform.GetChild(i).gameObject;

                // Deactivate the car and move it back to the inactive pool
                theCar.SetActive(false);
                theCar.transform.parent = unactiveCars.transform;
            }
        }
    }
}
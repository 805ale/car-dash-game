using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    // Array of possible path segment prefabs to spawn randomly
    [SerializeField] private GameObject[] pathPrefabs;

    // The first path segment already placed in the scene
    [SerializeField] private GameObject firstPath;

    // How many additional path segments to spawn after the first one
    [SerializeField] private int pathCount;

    // The length of each path piece along the Z-axis
    private float zPathSize;

    // Keeps track of all the spawned paths (including the first one)
    [SerializeField] private List<GameObject> pathList = new List<GameObject>();

    // Small positional offset between path pieces (useful if you need a gap or overlap)
    private const float positionBias = 0f; 

    // Called once when the game starts
    private void Start ()
    {
        // Measure the Z-size of the first path piece
        // Assumes that the visible mesh is on the first child object
        zPathSize = firstPath.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;

        // Add the initial path piece to the list
        pathList.Add(firstPath);

        // Spawn additional path pieces in front of the first one
        SpawnPath();
    }

    // Spawns a chain of path segments forward along the Z-axis
    private void SpawnPath ()
    {
        for (int i = 0; i < pathCount; i++)
        {
            // Determine the spawn position of the new piece:
            // Start from the last path pieces position and move forward by zPathSize
            Vector3 pathPosition = pathList[pathList.Count - 1].transform.position + Vector3.forward * zPathSize;

            // Apply a small Z offset if needed
            pathPosition.z += positionBias;

            // Pick a random path prefab from the array and instantiate it
            GameObject path = Instantiate(
                pathPrefabs[Random.Range(0, pathPrefabs.Length)],
                pathPosition,
                Quaternion.identity
            );

            path.transform.parent = transform;

            // Add the new path to the list for future positioning reference
            pathList.Add(path);
        }
    }
}
using UnityEngine;
using System.Collections;              // <-- Needed for IEnumerator / Coroutine
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    [Header("Prefabs & Initial Piece")]
    [SerializeField] private GameObject[] pathPrefabs;  // pool of possible segments
    [SerializeField] private GameObject firstPath;      // the first segment placed in scene

    [Header("Generation")]
    [SerializeField] private int pathCount = 12;        // how many more to pre-spawn after the first

    private float zPathSize;                            // measured length of one segment (along Z)
    public List<GameObject> pathList = new List<GameObject>();
    private const float positionBias = 0f;              // tiny overlap/gap adjustment if needed

    // index to the "oldest" segment we’ll recycle next
    public int listPathIndex = 0;                      // <-- int literal, not 0f

    public float destroyDistance;

    // reference the other car manager
    [SerializeField] OtherCarManager otherCarManager;

    private Camera mainCam;

    private void Start()
    {
        // Cache Camera.main once (faster than calling it every frame)
        mainCam = Camera.main;

        // Measure segment length from the first path’s visible mesh (assumes child 0 has Renderer)
        zPathSize = firstPath.transform.GetChild(0).GetComponent<Renderer>().bounds.size.z;

        firstPath.transform.SetParent(transform, true);

        // Seed the list with the first segment
        pathList.Add(firstPath);

        // Spawn additional segments forward
        SpawnPath();

        // Start recycling loop
        StartCoroutine(RepositionPath());               // <-- Correct StartCoroutine syntax
    }

    /// <summary>
    /// Pre-spawn a chain of path segments forward along +Z.
    /// </summary>
    private void SpawnPath()
    {
        for (int i = 0; i < pathCount; i++)
        {
            // Spawn position = last segment position + one segment length forward
            Vector3 pathPosition = pathList[pathList.Count - 1].transform.position + Vector3.forward * zPathSize;
            pathPosition.z += positionBias;

            // Pick a random prefab and spawn it
            GameObject prefab = pathPrefabs[Random.Range(0, pathPrefabs.Length)];
            GameObject path = Instantiate(prefab, pathPosition, Quaternion.identity);

            // Parent to this manager (keeps Hierarchy clean)
            path.transform.SetParent(transform, true);

            // Track it
            pathList.Add(path);
        }
    }

    /// <summary>
    /// Continuously recycles the oldest segment by moving it to the front
    /// once it’s far behind the camera/player.
    /// </summary>
    private IEnumerator RepositionPath()
    {
        while (true)
        {
            if (mainCam != null && pathList.Count > 0)
            {
                // When a segment is more than 15 units behind the camera, recycle it
                destroyDistance = mainCam.transform.position.z - 15f;

                // Check the oldest segment (at listPathIndex)
                if (pathList[listPathIndex].transform.position.z < destroyDistance)
                {
                    // Find the farthest-ahead segment and position the recycled one after it
                    Vector3 nextPathPos = FarthestPath().transform.position + Vector3.forward * zPathSize;
                    nextPathPos.z += positionBias;

                    // Move the recycled segment to the new spot (no destroy/instantiate GC)
                    pathList[listPathIndex].transform.SetPositionAndRotation(nextPathPos, Quaternion.identity);

                    otherCarManager.CheckAndDisableCarPath ();

                    // Advance the ring buffer index
                    listPathIndex++;
                    if (listPathIndex == pathList.Count)
                        listPathIndex = 0;
                }
            }

            otherCarManager.FindCarAndReset ();
            yield return null; // wait a frame
        }
    }

    /// <summary>
    /// Returns the segment with the greatest Z (the one farthest ahead).
    /// </summary>
    private GameObject FarthestPath()
    {
        GameObject farthest = pathList[0];
        float bestZ = farthest.transform.position.z;

        for (int i = 1; i < pathList.Count; i++)
        {
            float z = pathList[i].transform.position.z;
            if (z > bestZ)
            {
                bestZ = z;
                farthest = pathList[i];
            }
        }
        return farthest;
    }
}
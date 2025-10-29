using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Transform player;   // Assign your Player in Inspector
    [SerializeField] Rigidbody playerRb;
    [SerializeField, Min(0.01f)] private float smoothTime = 0.15f; // smaller = faster catch-up
    [SerializeField] private bool followY = true; // toggle if you only want X/Z follow

    private Vector3 offset;   // camera-to-player offset captured at start
    private Vector3 velocity; // required ref for SmoothDamp

    private void Awake()
    {
        if (!player)
        {
            // Optional: try to auto-find by tag to avoid null refs
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p) player = p.transform;
        }
    }

    private void Start()
    {
        if (!playerRb) playerRb = player.GetComponent<Rigidbody>();
        // Capture initial offset so camera keeps the same relative position
        offset = transform.position - player.position;

        if (!player)
        {
            Debug.LogError("[CameraMovement] No player assigned!");
            enabled = false;
            return;
        }
    }

    // Run AFTER player has moved this frame
    private void LateUpdate()
    {
        if (!player) return;

        // Use interpolated RB position when available
        Vector3 basePos = playerRb ? playerRb.position : player.position;
        Vector3 target = basePos + offset;
        if (!followY) target.y = transform.position.y;

        transform.position = Vector3.SmoothDamp(transform.position, target, ref velocity, smoothTime);
    }
}
using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

/// <summary>
/// Lane-based runner controller: swipe (or arrows in Editor) to dash between 5 lanes.
/// Applies a subtle lean during the dash, then returns to the prefab's neutral rotation.
/// </summary>
public class Player : MonoBehaviour
{
    // --- Lane world X positions ---
    public const float LeftLane2  = -8f;
    public const float LeftLane1  = -4f;
    public const float CenterLane =  0f;
    public const float RightLane1 =  4f;
    public const float RightLane2 =  8f;

    // Built from the constants at runtime
    private float[] lanePositions;
    // Current lane index (0..4). 0 = LeftLane2, 2 = Center, 4 = RightLane2
    private int laneIndex;

    // --- Touch swipe tracking ---
    private Vector2 startTouchPos;
    private Vector2 endTouchPos;
    [SerializeField] private float swipeThresholdPixels = 30f;

    // --- Optional physics tweak (kept from your version) ---
    Rigidbody rb;

    // --- Movement/Tilt tuning ---
    [Header("Dash Movement")]
    [SerializeField] private float moveDuration = 0.24f;
    [SerializeField] private Ease moveEase = Ease.InOutSine;

    [Header("Tilt While Dashing")]
    [SerializeField] private float yawTilt = 10f;   // Y (turn head)
    [SerializeField] private float rollTilt = 2f;  // Z (bank)
    [SerializeField] private float tiltInTime = 0.12f;
    [SerializeField] private float tiltBackTime = 0.14f;
    [SerializeField] private Ease tiltBackEase = Ease.InOutSine;

    [SerializeField] private float moveSpeed = 20f;

    [Header("Rotate Only Visual (Optional)")]
    [Tooltip("If set, only this transform will rotate for the lean (recommended: the car mesh). If null, the root rotates.")]
    [SerializeField] private Transform visualToRotate;

    private Transform RotTarget => visualToRotate != null ? visualToRotate : transform;

    // Store the prefab's neutral local rotation so we always tilt relative to it
    private Quaternion neutralLocalRot;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;   // smooth between physics ticks
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        if (rb) rb.centerOfMass = Vector3.zero;

        lanePositions = new float[] { LeftLane2, LeftLane1, CenterLane, RightLane1, RightLane2 };

        // Start on the nearest lane to our current X (no float == checks)
        laneIndex = NearestLaneIndex(transform.position.x);
        var p = transform.position;
        p.x = lanePositions[laneIndex];
        transform.position = p;

        // IMPORTANT: keep whatever local rotation the prefab already has
        neutralLocalRot = RotTarget.localRotation;
    }

    private void Update()
    {
        HandleSwipeInput();

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // Keyboard test controls in Editor
        if (Input.GetKeyDown(KeyCode.LeftArrow))  TryDash(-1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) TryDash(+1);
#endif
    }

    private void FixedUpdate()
    {
        // Move forward at a constant speed in physics space
        rb.linearVelocity = Vector3.forward * moveSpeed;
    }

    /// <summary>
    /// Reads touch and triggers a left/right dash when a horizontal swipe is detected.
    /// </summary>
    private void HandleSwipeInput()
    {
        if (Input.touchCount == 0) return;

        Touch t = Input.GetTouch(0);
        switch (t.phase)
        {
            case TouchPhase.Began:
                startTouchPos = t.position;
                break;

            case TouchPhase.Ended:
                endTouchPos = t.position;
                float dx = endTouchPos.x - startTouchPos.x;
                if (Mathf.Abs(dx) < swipeThresholdPixels) return;

                int dir = dx > 0 ? +1 : -1; // +1 right, -1 left
                TryDash(dir);
                break;
        }
    }

    /// <summary>
    /// Attempts to move one lane in 'dir'. If already at the edge, does nothing.
    /// </summary>
    private void TryDash(int dir)
    {
        int targetIndex = Mathf.Clamp(laneIndex + dir, 0, lanePositions.Length - 1);
        if (targetIndex == laneIndex) return;

        float targetX = lanePositions[targetIndex];
        MoveToLane(targetX, dir);
        laneIndex = targetIndex;
    }

    /// <summary>
    /// Core dash + tilt sequence. Kills any existing tweens (without completing) to avoid snaps.
    /// dir: -1 = left, +1 = right
    /// </summary>
    private void MoveToLane(float laneX, int dir)
    {
        // Prevent stacking tweens; don't complete them or you'll snap to end states
        transform.DOKill(false);
        RotTarget.DOKill(false);

        // Move across lanes
        var moveTween = transform.DOMoveX(laneX, moveDuration).SetEase(moveEase);

        // Tilt relative to the neutral rotation
        Quaternion tiltTarget = neutralLocalRot * Quaternion.Euler(0f,  yawTilt * dir, -rollTilt * dir);

        DOTween.Sequence()
               .Join(moveTween)
               .Join(RotTarget.DOLocalRotateQuaternion(tiltTarget, tiltInTime))
               .Append(RotTarget.DOLocalRotateQuaternion(neutralLocalRot, tiltBackTime).SetEase(tiltBackEase));
    }

    /// <summary>
    /// Finds the nearest lane index to a given X coordinate.
    /// </summary>
    private int NearestLaneIndex(float x)
    {
        int best = 0;
        float bestDist = Mathf.Abs(lanePositions[0] - x);
        for (int i = 1; i < lanePositions.Length; i++)
        {
            float d = Mathf.Abs(lanePositions[i] - x);
            if (d < bestDist)
            {
                best = i;
                bestDist = d;
            }
        }
        return best;
    }
}
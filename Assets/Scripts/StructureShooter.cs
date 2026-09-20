using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class StructureShooter : MonoBehaviour
{
    [Header("AR")]
    [SerializeField] private ARRaycastManager raycastManager;
    [SerializeField] private Camera arCamera;

    [Header("Structure")]
    [SerializeField] private GameObject structurePrefab;
    [SerializeField] private float minPlacementDistance = 1.2f; // ignore taps too close to the camera

    [Header("Shooting")]
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] private int startingAmmo = 8;
    [SerializeField] private float shootForce = 6f;
    [SerializeField] private float ballLifetime = 5f;
    [SerializeField] private float launchAngle = 12f; // upward tilt, so the ball arcs down onto the target

    public event System.Action<int> OnAmmoChanged;
    public event System.Action<int, int> OnBlocksChanged; // knocked, total
    public event System.Action<bool> OnGameOver; // true = win
    public event System.Action OnGameReset;

    private static readonly List<ARRaycastHit> hits = new List<ARRaycastHit>();

    private GameObject currentStructure;
    private readonly List<KnockableBlock> blocks = new List<KnockableBlock>();
    private int ammo;
    private int knockedCount;
    private bool structurePlaced;
    private bool isGameOver;

    private void Awake()
    {
        ammo = startingAmmo;
    }

    private void Update()
    {
        if (Touchscreen.current == null) return;
        if (!Touchscreen.current.primaryTouch.press.wasPressedThisFrame) return;

        int touchId = Touchscreen.current.primaryTouch.touchId.ReadValue();
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(touchId)) return; // ignore taps on UI buttons

        Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();

        if (isGameOver)
        {
            ResetGame();
            return;
        }

        if (!structurePlaced)
        {
            TryPlaceStructure(touchPosition);
        }
        else
        {
            Shoot();
        }
    }

    private void TryPlaceStructure(Vector2 screenPosition)
    {
        if (raycastManager == null || structurePrefab == null) return;
        if (!raycastManager.Raycast(screenPosition, hits, TrackableType.PlaneWithinPolygon)) return;

        Pose hitPose = hits[0].pose;

        if (Vector3.Distance(arCamera.transform.position, hitPose.position) < minPlacementDistance)
            return; // too close, ask the player to tap further away

        currentStructure = Instantiate(structurePrefab, hitPose.position, hitPose.rotation);
        structurePlaced = true;

        RegisterBlocks();
    }

    private void RegisterBlocks()
    {
        blocks.Clear();
        knockedCount = 0;

        foreach (KnockableBlock block in currentStructure.GetComponentsInChildren<KnockableBlock>())
        {
            blocks.Add(block);
            block.OnKnocked += HandleBlockKnocked;
        }

        OnBlocksChanged?.Invoke(knockedCount, blocks.Count);
        OnAmmoChanged?.Invoke(ammo);
    }

    private void Shoot()
    {
        if (ammo <= 0 || ballPrefab == null || arCamera == null) return;

        Vector3 shootDirection = Quaternion.AngleAxis(-launchAngle, arCamera.transform.right) * arCamera.transform.forward;
        Vector3 spawnPosition = arCamera.transform.position + shootDirection * 0.15f;

        GameObject ball = Instantiate(ballPrefab, spawnPosition, Quaternion.identity);
        Rigidbody rb = ball.GetComponent<Rigidbody>();
        rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);

        Destroy(ball, ballLifetime);

        ammo--;
        OnAmmoChanged?.Invoke(ammo);

        if (ammo <= 0 && knockedCount < blocks.Count)
        {
            // Give the last ball time to hit before declaring defeat
            Invoke(nameof(CheckOutOfAmmo), ballLifetime);
        }
    }

    private void CheckOutOfAmmo()
    {
        if (isGameOver) return;
        if (knockedCount < blocks.Count)
        {
            TriggerGameOver(false);
        }
    }

    private void HandleBlockKnocked()
    {
        knockedCount++;
        OnBlocksChanged?.Invoke(knockedCount, blocks.Count);

        if (knockedCount >= blocks.Count)
        {
            TriggerGameOver(true);
        }
    }

    private void TriggerGameOver(bool won)
    {
        if (isGameOver) return;
        isGameOver = true;
        CancelInvoke(nameof(CheckOutOfAmmo));
        OnGameOver?.Invoke(won);
    }

    public void ResetGame()
    {
        CancelInvoke(nameof(CheckOutOfAmmo));

        foreach (KnockableBlock block in blocks)
        {
            if (block != null) block.OnKnocked -= HandleBlockKnocked;
        }

        if (currentStructure != null) Destroy(currentStructure);

        blocks.Clear();
        knockedCount = 0;
        ammo = startingAmmo;
        structurePlaced = false;
        isGameOver = false;

        OnGameReset?.Invoke();
    }
}
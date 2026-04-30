using System;
using System.Collections.Generic;
using UnityEngine;

public enum TouchKey {Zero = 0, One  = 1, Two  = 2, Three = 3}

[Serializable]
public struct TouchMat
{
    public TouchKey key;
    public Material material;
}

public class Tile : BaseNPC
{
    [Header("Gameplay")]
    [SerializeField, Min(1)] private int touchCountTotal;
    [SerializeField] private int touchCountCurrent;
    [SerializeField] private float fallDelayTime = 0.15f;
    [SerializeField] private bool isFalling;
    [SerializeField] private float minimumYValue = -10f;

    [Header("Visuals")]
    [SerializeField] private Renderer targetRenderer;
    [SerializeField] private TouchMat[] palette;

    private readonly Dictionary<int, Material> lookup = new();
    private bool timerStarted;
    private Rigidbody rb;
    private HashSet<int> contactingPlayers = new();
    private HashSet<int> holdingPlayers = new();


    private int Remaining => Mathf.Clamp(touchCountTotal - touchCountCurrent, 0, touchCountTotal);

    protected override void Awake()
    {
        base.Awake();

        rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        // Defaults
        if (touchCountTotal <= 0) touchCountTotal = 1;
        if (touchCountCurrent < 0) touchCountCurrent = 0;

        RebuildLookup();
        ApplyMaterial();
    }

    protected void Update()
    {
        if (isFalling && transform.position.y <= minimumYValue)
        {
            Destroy(gameObject);
        }
    }

void OnCollisionEnter(Collision collision)
{
    if (!collision.transform.root.CompareTag("Player")) return;
    if (timerStarted) return;

    var player = collision.transform.GetComponentInParent<PlayerController>();
    int id = collision.transform.root.GetInstanceID();
    
    if (player != null && player.IsHolding) return;

    if (contactingPlayers.Add(id))
    {
        touchCountCurrent++;
        ApplyMaterial();
       
        if (touchCountCurrent > touchCountTotal)
            StartFall();
    }
}

void OnCollisionExit(Collision collision)
{
    if (!collision.transform.root.CompareTag("Player")) return;

    var player = collision.transform.GetComponentInParent<PlayerController>();
    int id = collision.transform.root.GetInstanceID();

    if (player != null && player.IsHolding)
    {
        // Remember this player was holding when they exited
        holdingPlayers.Add(id);
        return;
    }

    // If this player was holding when they last exited, skip this exit too
    if (holdingPlayers.Remove(id)) return;

    if (contactingPlayers.Remove(id))
    {
        if (!timerStarted && touchCountCurrent >= touchCountTotal && contactingPlayers.Count == 0)
            StartFall();
    }
}


    void Fall()
    {
        if (!rb) rb = GetComponent<Rigidbody>();
        if (!rb) return;

        rb.constraints =
            RigidbodyConstraints.FreezePositionX |
            RigidbodyConstraints.FreezePositionZ |
            RigidbodyConstraints.FreezeRotation;

        rb.useGravity = true;
    }

    void ApplyMaterial()
    {
        if (!targetRenderer) return;

        Material mat = null;
        for (int r = Remaining; r >= 0; r--)
        {
            if (lookup.TryGetValue(r, out mat))
                break;
        }

        if (mat != null)
        {
            targetRenderer.sharedMaterial = mat;
        }
    }

    void RebuildLookup()
    {
        lookup.Clear();
        if (palette == null) return;

        foreach (var p in palette)
        {
            if (p.material != null)
                lookup[(int)p.key] = p.material;
        }
    }

    public void StartFall(float? delayOverride = null)
    {
        if (timerStarted) return;

        timerStarted = true;
        isFalling = true;
        ApplyMaterial();

        float delay = delayOverride ?? fallDelayTime;
        if (delay <= 0f) Fall();
        else Invoke(nameof(Fall), delay);
    }
}

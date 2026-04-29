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
    private int activeContacts = 0;


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

    protected override void Update()
    {
        base.Update();

        if (isFalling && transform.position.y <= minimumYValue)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.transform.root.CompareTag("Player")) return;
        if (timerStarted) return;

        activeContacts++;

        // Increment once; clamp to avoid overshoot if multiple colliders fire
        touchCountCurrent = touchCountCurrent + 1;
        // Update visual each time the count changes
        ApplyMaterial();

        // To prevent second player exploit - tile will fall immediatly if two players try to occupy a tile on its last life.
        if (touchCountCurrent > touchCountTotal)
        {
            StartFall();
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (!collision.transform.root.CompareTag("Player")) return;

        activeContacts = Mathf.Max(0, activeContacts - 1);

        // If tile touchcount <= 0 and no players are touching it - Fall
        if (!timerStarted && touchCountCurrent >= touchCountTotal && activeContacts <= 0)
        {
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

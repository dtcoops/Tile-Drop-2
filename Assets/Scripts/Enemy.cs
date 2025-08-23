using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseNPC
{
    [Header("Explosion")]
    [SerializeField] float explodeRadius   = 3f;
    [SerializeField] float yBand           = 0.4f;        // vertical thickness around platform
    [SerializeField] LayerMask tileMask;                  // set to your Tile layer
    [SerializeField] float tileFallDelay   = 0.15f;
    [SerializeField] bool  destroySelf     = true;
    [SerializeField] float selfDestructDelay = 0.05f;

    private bool exploded;

    protected override void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // prevent refiring
        if (exploded)
        {
            return;
        }
        // check parent tag (treat colliders on children as one entity)
        if (collision.transform.root.CompareTag("Player"))
        {
            // Explode --- explosion animation? -- destroy block on same Y coordinate in a radius
            Explode();
        }
    }
    void Explode()
    {
        if (exploded) { return; }
        exploded = true;

        // 1) Find the platform Y under the enemy
        float platformY = transform.position.y;
        if (Physics.Raycast(
            transform.position + Vector3.up * 0.1f,
            Vector3.down,
            out var hit,
            20f,
            tileMask,
            QueryTriggerInteraction.Collide)
            )
        {
            platformY = hit.point.y;
        }

        // 2) Overlap a thin box around that Y to catch only same-level tiles
        var center      = new Vector3(transform.position.x, platformY, transform.position.z);
        var halfExtents = new Vector3(explodeRadius, yBand * 0.5f, explodeRadius);

        var hits = Physics.OverlapBox(center, halfExtents, Quaternion.identity,
                                      tileMask, QueryTriggerInteraction.Collide);

        // 3) De-dupe by Tile instance (NOT by root)
        var seenTiles = new HashSet<int>(); // instance IDs
        int touched = 0;
        foreach (var col in hits)
        {
            // de-dupe by tile root
            var tile = col.GetComponentInParent<Tile>();
            if (!tile) continue;

            int id = tile.GetInstanceID();
            if (!seenTiles.Add(id)) continue;

            tile.ForceFall(tileFallDelay);
            touched++;
        }

        // Prevent retriggering
        var myCol = GetComponent<Collider>();
        if (myCol) myCol.enabled = false;
        if (destroySelf) Destroy(gameObject, selfDestructDelay);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        // visualize the overlap box
        Gizmos.color = new Color(1f, 0.5f, 0f, 0.25f);
        // Try to draw roughly where it will be (uses current position Y as a guess)
        var center = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        var halfExtents = new Vector3(explodeRadius, yBand * 0.5f, explodeRadius);
        Gizmos.DrawCube(center, halfExtents * 2f);
    }
#endif
}
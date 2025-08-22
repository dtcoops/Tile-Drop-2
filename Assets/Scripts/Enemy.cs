using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : BaseNPC
{
    protected override void Awake()
    {
        base.Awake();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            // Explode --- explosion animation? -- destroy block on same Y coordinate in a radius
        }
    }

    void Explode()
    {
        GetComponent<SphereCollider>().radius = 3;
    }
}

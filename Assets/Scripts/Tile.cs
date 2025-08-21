using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int touchCountTotal;
    [SerializeField] private int touchCountCurrent;
    [SerializeField] private float fallDelayTime;
    [SerializeField] private bool isFalling;
    [SerializeField] private float minimumYValue; // Cannot fall below

    private bool timerStarted = false;
    private Rigidbody rb;

    public void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeAll;

        touchCountTotal = touchCountTotal != 0 ? touchCountTotal : 1;
        touchCountCurrent = touchCountCurrent != 0 ? touchCountCurrent : 0;
    }
    private void Update()
    {
        if (isFalling)
        {
            if (transform.position.y <= minimumYValue)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player")) return;
        if (timerStarted) return;

        touchCountCurrent++;

        if (!timerStarted && touchCountCurrent >= touchCountTotal)
        {
            timerStarted = true;
            isFalling = true;
            Invoke(nameof(Fall), fallDelayTime);
        }
        
        return;
    }

    private void Fall()
    {
        GetComponent<Rigidbody>().constraints =
         RigidbodyConstraints.FreezePositionZ |
         RigidbodyConstraints.FreezePositionX |
         RigidbodyConstraints.FreezeRotation;
        GetComponent<Rigidbody>().useGravity = true;
    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField]
    private int touchCountTotal;
    [SerializeField]
    private int touchCountCurrent;
    [SerializeField]
    private float fallDelayTime;
    [SerializeField]
    private bool isFalling;
    [SerializeField]
    private float minimumYValue; // Cannot fall below

    private void Update()
    {
        if (isFalling)
        {
            if(transform.position.y <= minimumYValue)
            {
                Destroy();
            }
        }
    }
    void BeginFall()
    {
        isFalling = true;
        Invoke("Fall", fallDelayTime);
    }

    private void Fall()
    {
        GetComponent<Rigidbody>().useGravity = true;
    }

    void Destroy()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNPC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField]
    protected bool isMovable = false;
    [SerializeField]
    protected List<Transform> patrolPoints;
    [SerializeField]
    protected float speed = 1.0f;

    protected float positionMarginOfError = 0.05f;
    protected int targetLocationIndex = 0;

    protected virtual void Awake()
    {
        if (patrolPoints != null && patrolPoints.Count > 0)
            targetLocationIndex = 0;
    }

    protected virtual void Update()
    {
        if (isMovable && patrolPoints != null && patrolPoints.Count > 0)
            PatrolStep();
    }

    protected virtual void PatrolStep()
    {
        var target = patrolPoints[targetLocationIndex];
        transform.position = Vector3.MoveTowards(
            transform.position, target.position, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target.position) <= positionMarginOfError)
            targetLocationIndex = (targetLocationIndex + 1) % patrolPoints.Count;
    }
}
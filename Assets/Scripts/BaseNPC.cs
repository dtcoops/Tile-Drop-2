using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseNPC : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] protected bool isMovable = false;
    [SerializeField] protected List<Transform> patrolPoints;
    [SerializeField] protected float speed = 1.0f;
    [SerializeField] private float pauseDuration = 1f;
    
    [Header("Synchronization")]
    [SerializeField] private float cyclePeriod = 10f;

    protected float positionMarginOfError = 0.05f;
    protected int targetLocationIndex = 0;

    protected virtual void Awake()
    {
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            speed = CalculateSpeed();
            targetLocationIndex = 0;
            StartCoroutine(PatrolWithPause());
        }
    }

    private float CalculateSpeed()
    {
        float totalDistance = 0f;
        for (int i = 0; i < patrolPoints.Count; i++)
        {
            int next = (i + 1) % patrolPoints.Count;
            totalDistance += Vector3.Distance(patrolPoints[i].position, patrolPoints[next].position);
        }

        float totalPauseTime = pauseDuration * patrolPoints.Count;
        float movingTime = cyclePeriod - totalPauseTime;

        return totalDistance / movingTime;
    }

    protected virtual IEnumerator PatrolWithPause()
    {
        while (true)
        {
            var target = patrolPoints[targetLocationIndex];

            while (Vector3.Distance(transform.position, target.position) > positionMarginOfError)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
                yield return null;
            }

            // Snap to exact position
            transform.position = target.position;
            targetLocationIndex = (targetLocationIndex + 1) % patrolPoints.Count;

            yield return new WaitForSeconds(pauseDuration);
        }
    }
}
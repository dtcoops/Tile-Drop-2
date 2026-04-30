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

    protected float positionMarginOfError = 0.05f;
    protected int targetLocationIndex = 0;

    protected virtual void Awake()
    {
        if (patrolPoints != null && patrolPoints.Count > 0)
        {
            targetLocationIndex = 0;
            StartCoroutine(PatrolWithPause());
        }
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
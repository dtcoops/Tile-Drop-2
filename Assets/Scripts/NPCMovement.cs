using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    private int targetLocationIndex = 0;

    private float positionMarginOfError;


    public void Move(Transform currentLocation, List<Transform> patrolPoints, float speed)
    {
        while (currentLocation.position != patrolPoints[targetLocationIndex].position) {
            if (Vector3.Distance(currentLocation.position, patrolPoints[targetLocationIndex].position) < positionMarginOfError)
            {
                targetLocationIndex++;
                if (targetLocationIndex >= patrolPoints.Count)
                {
                    targetLocationIndex = 0;
                }
            }

            gameObject.transform.position = Vector3.MoveTowards(currentLocation.position, patrolPoints[targetLocationIndex].position, speed * Time.deltaTime);
        }
    }

}

// Vector3.Distance(obj.transform.position, objToCompare.transform.position) < "Margin of difference"
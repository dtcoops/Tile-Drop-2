using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    [SerializeField]
    private List<Transform> patrolPoints;
    [SerializeField]
    private Transform currentLocation;
    [SerializeField]
    private Transform targetLocation;

}

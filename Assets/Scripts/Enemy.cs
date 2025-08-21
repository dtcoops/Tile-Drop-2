using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private List<Transform> patrolPoints;
    [SerializeField]
    private float speed;

    private float positionMarginOfError;
    private int lastVisitedIndex;
    private int targetLocationIndex;

    void Start()
    {
        positionMarginOfError = .05f;
        lastVisitedIndex = 0;
        targetLocationIndex = 1;

        Transform enemy = GameObject.FindWithTag("Enemy").transform;
        Debug.Log(enemy, enemy);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    public void Move()
    {
        Debug.Log(Vector3.Distance(this.transform.position, patrolPoints[targetLocationIndex].position));
        Debug.Log(positionMarginOfError);
        if (Vector3.Distance(gameObject.transform.position, patrolPoints[targetLocationIndex].position) <= positionMarginOfError)
        {
            targetLocationIndex++;
            lastVisitedIndex++;
            if (targetLocationIndex >= patrolPoints.Count)
            {
                targetLocationIndex = 0;
            }
            if (lastVisitedIndex >= patrolPoints.Count)
            {
                lastVisitedIndex = 0;
            }
        }
        gameObject.transform.position = Vector3.MoveTowards(gameObject.transform.position, patrolPoints[targetLocationIndex].position, speed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            // Explode --- explosion animation? -- destroy block on same Y coordinate in a radius
        }
    }

    void Explode()
    {
        GetComponent<SphereCollider>().radius = 3;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float playerMoveSpeed;
    [SerializeField]
    private float playerJumpForce;

    private bool isGrounded;

    // Update is called once per frame
    void Update()
    {
        // horizontal and vertical inputs
        float xMovement = Input.GetAxis("Horizontal") * playerMoveSpeed;
        float zMovement = Input.GetAxis("Vertical") * playerMoveSpeed;

        rb.velocity = new Vector3(xMovement, rb.velocity.y, zMovement);

        // copy velocity and set y to 0 to prvent rotating as it falls
        Vector3 vel = rb.velocity;
        vel.y = 0;
        // use new velocity to rotate to moving direction
        if (vel.x != 0 || vel.z != 0)
        {
            transform.forward = vel;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
            isGrounded = false;
        }

        if (transform.position.y <= -10)
        {

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile"))
        {
            isGrounded = true;
        }
    }


}

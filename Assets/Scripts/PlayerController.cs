using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerSide { Left, Right }

public class PlayerController : MonoBehaviour
{
    [Header("Identity")]
    [SerializeField] private PlayerSide side = PlayerSide.Left;

    [Header("Movement")]
    [SerializeField]
    private Rigidbody rb;
    [SerializeField]
    private float playerMoveSpeed;
    [SerializeField]
    private float playerJumpForce;

    private bool isGrounded;

    void Awake()
    {
        if (!rb)
        {
            rb = GetComponent<Rigidbody>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        bool held = (side == PlayerSide.Left) ? Input.GetMouseButton(0) : Input.GetMouseButton(1);

        float xMovement = 0f;
        float zMovement = 0f;
        // horizontal and vertical inputs
        if (held)
        {
            xMovement = Input.GetAxis("Horizontal") * playerMoveSpeed;
            zMovement = Input.GetAxis("Vertical") * playerMoveSpeed;

            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                Jump();
            }
        }

        rb.velocity = new Vector3(xMovement, rb.velocity.y, zMovement);

        // copy velocity and set y to 0 to prvent rotating as it falls
        Vector3 vel = rb.velocity;
        vel.y = 0;
        // use new velocity to rotate to moving direction
        if (vel.x != 0 || vel.z != 0)
        {
            transform.forward = vel;
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

    private void Jump()
    {
        rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
        isGrounded = false;
    }


}

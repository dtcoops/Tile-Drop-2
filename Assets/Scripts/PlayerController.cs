using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum PlayerSide { Left, Right }
public enum PlayerRole { Jumper, Catcher }

public class PlayerController : MonoBehaviour
{
    // Events
    public static event Action OnPlayerDied;

    [Header("Identity")]
    [SerializeField] private PlayerSide side = PlayerSide.Left;
    [SerializeField] private PlayerRole role;
    [SerializeField] private PlayerController partner;

    [Header("Movement")]
    [SerializeField] private Rigidbody rb;
    [SerializeField] private float playerMoveSpeed;
    [SerializeField] private float playerJumpForce;
    [SerializeField] private float minimumYValue = -10f;

    [Header("Throw")]
    [SerializeField] private float throwDelay = 0.5f;
    [SerializeField] private float throwDistance = 1.4f;
    [SerializeField] private float throwHeight = 1.0f;

    private bool isGrounded;
    private bool isCaught;
    private bool isHolding;
    private bool isBeingThrown;
    private Vector3 throwDirection;
    private Vector3 lastMoveDirection;

    public bool IsHolding => isHolding;

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
        if (transform.position.y <= minimumYValue)
        {
            StopAllCoroutines();
            OnPlayerDied?.Invoke();
            Destroy(gameObject);
        }

        // No input if caught or holding
        if (isCaught || isHolding || isBeingThrown) return;

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

        rb.linearVelocity = new Vector3(xMovement, rb.linearVelocity.y, zMovement);

        // copy velocity and set y to 0 to prvent rotating as it falls
        Vector3 vel = rb.linearVelocity;
        vel.y = 0;
        // use new velocity to rotate to moving direction
        if (vel.x != 0 || vel.z != 0)
        {
            transform.forward = vel;
            lastMoveDirection = vel.normalized;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check - Grounded
        if (collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Ground"))
        {
            // Don't interrupt a throw in progress
            if (isBeingThrown)
            {
                isGrounded = true;
                isBeingThrown = false;
                return;
            }

            isGrounded = true;

            // Safety Catch - prevent landing on the ground without controls
            if (isCaught)
            {
                isCaught = false;
                rb.isKinematic = false;
            }
        }

        // Check - Lands on Catcher
        if (role == PlayerRole.Jumper && partner != null && collision.transform.root == partner.transform.root)
        {   
            if (isBeingThrown) return;

            // Only catch if Jumper is above Catcher
            if (transform.position.y < partner.transform.position.y + 0.5f) return;

            // Check - Can Catch
            if (!isCaught && !partner.IsHolding)
            {
                // Jumper is Caught, tell Catcher they are now holding Jumper
                partner.StartHolding();

                // Need to store Jumpers horizontal velocity and direction for throw 
                throwDirection = lastMoveDirection;

                // Snap Jumper to Catcher - prevent Jumper input
                isCaught = true;
                rb.isKinematic = true;
                transform.SetParent(partner.transform);
                transform.localPosition = new Vector3(0, 1.1f, 0);

                // Throw is automated
                StartCoroutine(ThrowSequence());

            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Tile") || collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    private void Jump()
    {
        rb.AddForce(Vector3.up * playerJumpForce, ForceMode.Impulse);
        isGrounded = false;
    }
    
    public void StartHolding()
    {
        isHolding = true;
        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    public void StopHolding()
    {
        isHolding = false;
        rb.isKinematic = false;
    }

    IEnumerator ThrowSequence()
    {
        yield return new WaitForSeconds(throwDelay);

        // Un-Snap Jumper
        transform.SetParent(null);
        isCaught = false;
        isBeingThrown = true;
        rb.isKinematic = false;

        Physics.IgnoreCollision(GetComponent<Collider>(), partner.GetComponent<Collider>(), true);

        // Launch from same height as target to keep arc calculation clean
        transform.position = new Vector3(
            partner.transform.position.x,
            partner.transform.position.y + 0.5f,
            partner.transform.position.z
        );

        Vector3 targetPosition = partner.transform.position + throwDirection * throwDistance;

        Vector3 launchVelocity = CalculateLaunchVelocity(transform.position, targetPosition, throwHeight);
        rb.linearVelocity = launchVelocity;

        StartCoroutine(ReEnableCollision(partner.GetComponent<Collider>()));

        // Release Catcher
        partner.StopHolding();
    }

    IEnumerator ReEnableCollision(Collider partnerCollider)
    {
        yield return new WaitForSeconds(0.5f);
        Physics.IgnoreCollision(GetComponent<Collider>(), partnerCollider, false);
    }

    private Vector3 CalculateLaunchVelocity(Vector3 origin, Vector3 target, float height)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float displacementY = target.y - origin.y;
        Vector3 displacementXZ = new Vector3(target.x - origin.x, 0, target.z - origin.z);

        float timeUp = Mathf.Sqrt(2 * height / gravity);
        float timeDown = Mathf.Sqrt(2 * (height - displacementY) / gravity);
        float totalTime = timeUp + timeDown;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(2 * gravity * height);
        Vector3 velocityXZ = displacementXZ / totalTime;

        return velocityXZ + velocityY;
    }
}

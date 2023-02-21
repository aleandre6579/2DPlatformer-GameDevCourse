using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Components
    private Rigidbody2D rb;

    // Movement
    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float linearDrag;
    [SerializeField] private float jumpForce;

    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    // Booleans
    [SerializeField] private bool isGrounded;

    // Checks
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    // Inputs
    private PlayerInputActions playerActions;
    private Vector2 moveDirection;
    private InputAction move;
    private InputAction jump;
    private InputAction fire;
    private bool changingDirection => (rb.velocity.x > 0f && moveDirection.x < 0f) || (rb.velocity.x < 0f && moveDirection.x > 0f);

    // Animation
    private Animator playerAnim;

    private void OnEnable()
    {
        move = playerActions.Player.Move;
        move.Enable();

        jump = playerActions.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        fire = playerActions.Player.Fire;
        fire.Enable();
        fire.performed += Fire;
    }

    private void OnDisable()
    {
        move.Disable();
        fire.Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerActions = new PlayerInputActions();
        playerAnim = GetComponent<Animator>();
    }

    private void Update()
    {
        Move();
        Fall();
        CheckGrounded();
    }

    private void CheckGrounded()
    {
        if (Physics2D.OverlapCircle(new Vector2(groundCheckPoint.position.x, groundCheckPoint.position.y), groundCheckRadius, groundLayer))
            isGrounded = true;
        else
            isGrounded = false;
    }

    private void Fall()
    {
        if (rb.velocity.y < -0.1f)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
        else if (rb.velocity.y > 0 && !jump.inProgress)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
        }
    }

    private void Move()
    {
        moveDirection = move.ReadValue<Vector2>();

        // Move the player
        float movement = moveDirection.x * speed;
        rb.AddForce(Vector2.right * movement * Time.deltaTime, ForceMode2D.Impulse);
        if (Mathf.Abs(rb.velocity.x) > maxSpeed)
            rb.velocity = new Vector2(Mathf.Sign(rb.velocity.x) * maxSpeed, rb.velocity.y);

        // Animate the movement
        if(Mathf.Abs(rb.velocity.x) > 0)
            playerAnim.SetBool("running", true);
        else
            playerAnim.SetBool("running", false);

        // Add drag to the movement
        if((!move.inProgress && Mathf.Abs(rb.velocity.x) > 0.1f) || (changingDirection && isGrounded))
        {
            rb.drag = linearDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        // Flip the character based on direction
        if (moveDirection.x >= 0)
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        else            
            transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));

    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }
    private void Fire(InputAction.CallbackContext context)
    {
        playerAnim.SetTrigger("shoot");
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(groundCheckPoint.position, transform.forward, groundCheckRadius);
    }

}

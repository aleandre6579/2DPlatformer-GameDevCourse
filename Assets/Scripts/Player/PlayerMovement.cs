using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float superJumpForce;
    private float normalJumpForce;
    [SerializeField] private float landForce;

    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float lowJumpMultiplier = 2f;

    // Booleans
    [SerializeField] private bool isGrounded;
    private bool prevGrounded;
    [SerializeField] private bool isWallSliding;
    [SerializeField] private bool horMovePressed;
    public bool atGate = false;

    // Checks
    [SerializeField] private Transform groundCheckPoint;
    [SerializeField] private Transform leftWallCheckPoint;
    [SerializeField] private Transform rightWallCheckPoint;
    [SerializeField] private float checkRadius;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask jumpBlockLayer;

    // Inputs
    private PlayerInputActions playerActions;
    private Vector2 moveDirection;
    private InputAction move;
    private InputAction jump;
    private InputAction enter;
    private bool changingDirection => (rb.velocity.x > 0f && moveDirection.x < 0f) || (rb.velocity.x < 0f && moveDirection.x > 0f);

    // Animation
    private Animator playerAnim;
    [SerializeField] private Animator panelAnim;

    // Particles
    [SerializeField] private GameObject dashEffect;

    private void OnEnable()
    {
        move = playerActions.Player.Move;
        move.Enable();
        move.started += OnMove;

        jump = playerActions.Player.Jump;
        jump.Enable();
        jump.performed += Jump;

        enter = playerActions.Player.Enter;
        enter.Enable();
        enter.performed += Enter;
    }

    private void OnDisable()
    {
        move.Disable();
        jump.Disable();
        enter.Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        playerActions = new PlayerInputActions();
        playerAnim = GetComponent<Animator>();
    }

    private void Start()
    {
        normalJumpForce = jumpForce;
    }

    private void Update()
    {
        CheckJumpBlock();
        Move();
        Fall();
        CheckGrounded();
    }

    private void CheckJumpBlock()
    {
        if (Physics2D.OverlapCircle(new Vector2(groundCheckPoint.position.x, groundCheckPoint.position.y), checkRadius, jumpBlockLayer))
        {
            jumpForce = superJumpForce;
        }
        else
        {
            jumpForce = normalJumpForce;
        }
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        if(context.control.name == "a" || context.control.name == "d")
        {
            horMovePressed = true;
        }
    }

    private void LateUpdate()
    {
        horMovePressed = false;
    }

    private void CheckGrounded()
    {
        prevGrounded = isGrounded;
        if (Physics2D.OverlapCircle(new Vector2(groundCheckPoint.position.x, groundCheckPoint.position.y), checkRadius, groundLayer))
            isGrounded = true;
        else
            isGrounded = false;

        if (Physics2D.OverlapCircle(new Vector2(leftWallCheckPoint.position.x, leftWallCheckPoint.position.y), checkRadius, groundLayer) ||
            Physics2D.OverlapCircle(new Vector2(rightWallCheckPoint.position.x, rightWallCheckPoint.position.y), checkRadius, groundLayer))
            isWallSliding = true;
        else
            isWallSliding = false;

    }

    // Gives the player an extra short period to apply boost from direction change on landing
    private IEnumerator GroundedDelay()
    {
        float elapsedTime = 0;
        while(elapsedTime < 0.2f)
        {
            if (horMovePressed)
            {
                ChangeDirectionDash();
                yield break;
            }
            yield return null;
            elapsedTime += Time.deltaTime;
        }
    }

    private void ChangeDirectionDash()
    {
        rb.velocity += moveDirection * Vector2.right * landForce;
        GameObject inst;
        if (transform.rotation.y == 0)
            inst = Instantiate(dashEffect, groundCheckPoint.transform.position, dashEffect.transform.rotation);
        else
            inst = Instantiate(dashEffect, groundCheckPoint.transform.position, Quaternion.Euler(0, 90, 0));
        inst.transform.parent = transform;
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
        if(Mathf.Abs(moveDirection.x) > 0.1f)
            playerAnim.SetBool("running", true);
        else
            playerAnim.SetBool("running", false);

        // Add drag to the movement
        if(isGrounded && moveDirection.x < 0.1f && moveDirection.x > -0.1f)
        {
            rb.drag = linearDrag;
        }
        else
        {
            rb.drag = 0f;
        }

        // When the player lands add a force on the player towards their movement direction
        if (isGrounded && !prevGrounded)
        {
            if (Mathf.Abs(moveDirection.x) > 0.1f) rb.velocity += new Vector2(Mathf.Clamp(Mathf.Abs(rb.velocity.x), 0.001f, float.PositiveInfinity) / (moveDirection.x * landForce), 0);
            StartCoroutine(GroundedDelay());
        }

        // Flip the character based on direction
        if (moveDirection.x > 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
         else if(moveDirection.x < 0)
                transform.rotation = Quaternion.Euler(new Vector3(0, 180, 0));


    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (!isGrounded) return;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void Enter(InputAction.CallbackContext context)
    {
        if(atGate)
        {
            panelAnim.SetBool("fadeIn", true);
            Invoke("NextLevel", 1.2f);
        }
    }

    private void NextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }

    private void OnDrawGizmos()
    {
        Handles.color = Color.green;
        Handles.DrawWireDisc(groundCheckPoint.position, transform.forward, checkRadius);
        Handles.DrawWireDisc(leftWallCheckPoint.position, transform.forward, checkRadius);
        Handles.DrawWireDisc(rightWallCheckPoint.position, transform.forward, checkRadius);
    }

}

using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;
    public Transform groundCheck;
    public float groundRadius = 0.2f;
    public LayerMask groundLayer;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public Transform wallCheck;
    public float wallCheckDistance = 0.5f;
    public LayerMask wallLayer;
    public float wallSlideSpeed = 2f;
    public float wallJumpForce = 12f;
    public float wallJumpPush = 7f;
    private float lastDirection = 1f;
    private float wallJumpTime = 0.2f;
    private float wallJumpCounter;
    private float jumpBufferCounter;
    private float coyoteCounter;


    private float move;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isWallJumping;
    private bool running;
    private bool isGrounded;
    public bool isFacingRight = true;
    public bool isAttacking;
    public bool isHurt;
    public bool isDead;
    private Rigidbody2D rb;
    private Animator animator;

    void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError("RigidBody2D tidak ditemukan!!");
        }
    }

    void Update()
    {
        if (!isAttacking && !isHurt && !isDead)
        {
            move = Input.GetAxis("Horizontal");
        }

        animator.SetFloat("Speed", Mathf.Abs(move));

        animator.SetBool("IsRunning", running && Mathf.Abs(move) > 0.1f);

        if (move > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (move < 0 && isFacingRight)
        {
            Flip();
        }

        running = Input.GetKey(KeyCode.LeftShift);

        isGrounded = Physics2D.OverlapCircle(
            groundCheck.position,
            groundRadius,
            groundLayer
        );

        Debug.DrawRay(
            wallCheck.position,
            Vector2.right * lastDirection * wallCheckDistance,
            Color.blue
        );

        if (move != 0)
        {
            lastDirection = Mathf.Sign(move);
        }

        isTouchingWall = Physics2D.Raycast(
            wallCheck.position,
            Vector2.right * lastDirection,
            wallCheckDistance,
            wallLayer
        );


        if (isGrounded)
        {
            coyoteCounter = coyoteTime;
        }
        else
        {
            coyoteCounter -= Time.deltaTime;
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (isTouchingWall && !isGrounded && rb.linearVelocity.y < 0 && move != 0)
        {
            isWallSliding = true;
        }
        else
        {
            isWallSliding = false;
        }

        if (Input.GetKeyDown(KeyCode.Space) && isTouchingWall && !isGrounded)
        {
            isWallJumping = true;
            wallJumpCounter = wallJumpTime;
            rb.linearVelocity = new Vector2(
                -lastDirection * wallJumpPush,
                wallJumpForce
            );
        }

        if (isWallJumping)
        {

            wallJumpCounter -= Time.deltaTime;

            if (wallJumpCounter <= 0)
            {
                isWallJumping = false;
            }
        }

    }

    void FixedUpdate()
    {
        float currentSpeed = speed;

        if (running)
        {
            currentSpeed *= 2;
        }

        if (!isWallJumping && !isAttacking && !isHurt)
        {
            rb.linearVelocity = new Vector2(move * currentSpeed, rb.linearVelocity.y);
        }

        if (jumpBufferCounter > 0 && coyoteCounter > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            jumpBufferCounter = 0;
            coyoteCounter = 0;
        }

        if (!isWallSliding)
        {
            if (rb.linearVelocity.y < 0)
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
            {
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        if (isWallSliding)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)
            );
        }

    }
}
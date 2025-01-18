using System;
using JetBrains.Annotations;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private bool facingRight = false;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;

    public int jumpAmount;
    private int jumpAmountRemaining;
    private int facingDirection = -1;

    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask Ground;

    private float horizontal;
    public float movespeed;
    public float jumpStrength;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    private float ticker;
    public float updateTime;

    public Vector2 wallJumpStrength = new Vector2(10f, 0.3f);

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;

    public TMPro.TMP_Text varDisplay;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        JumpCheck();
        WallSlideCheck();
        CheckDirection();
        CheckForInput();
        UpdateAnimations();
        displayVariableOnScreen(rb.linearVelocity.x);
    }

    private void FixedUpdate()
    {
        Move();
        CheckSurroundings();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", rb.linearVelocity.x > 0.01f || rb.linearVelocity.x < -0.01f);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocityY);
        anim.SetBool("isWallSliding", isWallSliding);
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Ground);

        isTouchingWall = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, Ground);
    }

    private void CheckForInput()
    {
        horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }


        if (Input.GetButtonUp("Jump") && rb.linearVelocityY > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.5f);
        }
    }

    private void Move()
    {
        rb.linearVelocity = new Vector2(movespeed * horizontal, rb.linearVelocityY);

        if (isWallSliding)
        {
            if(rb.linearVelocityY < -wallSlideSpeed)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocityX, -wallSlideSpeed);
            }
        }
    }

    private void Jump()
    {
        if (jumpAmountRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
            jumpAmountRemaining--;
        }
    }

    private void CheckDirection()
    {
        if (!facingRight && horizontal < 0)
        {
            Flip();
        }

        else if (facingRight && horizontal > 0)
        {
            Flip();
        }
    }

    private void JumpCheck()
    {
        if (isGrounded && rb.linearVelocityY < 0.001f)
        {
            jumpAmountRemaining = jumpAmount;
        }

        if(isWallSliding && jumpAmountRemaining <= 1)
        {
            jumpAmountRemaining = 1;
        }
    }

    private void WallSlideCheck()
    {
        isWallSliding = (isTouchingWall && !isGrounded && rb.linearVelocityY < 0);
    }

    private void Flip()
    {   
        if (!isWallSliding)
        {
            facingDirection *= -1; 
            facingRight = !facingRight;
            sprite.transform.Rotate(0, 180, 0);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheck.position, new Vector3(wallCheck.position.x + wallCheckDistance, wallCheck.position.y, wallCheck.position.z));
    }

    private void displayVariableOnScreen(float variable)
    {
        ticker += Time.deltaTime;

        if (ticker > updateTime)
        {
            ticker -= updateTime;
            varDisplay.text = variable.ToString("F3");
        }
    }
}

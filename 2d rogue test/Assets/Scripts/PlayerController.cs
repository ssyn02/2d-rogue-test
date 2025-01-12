using System;
using UnityEditor.Tilemaps;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float horizontal;
    private bool facingRight = false;
    public bool isGrounded;

    public int jumpAmount = 1;
    private int jumpAmountRemaining;

    public Transform groundCheck;
    public LayerMask Ground;

    public float movespeed = 5f;
    public float jumpStrength = 16f;
    public float groundCheckRadius;

    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    
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
        CheckForInput();
        CheckDirection();
        UpdateAnimations();
        JumpCheck();
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
    }

    private void CheckSurroundings()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, Ground);
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
    }

    private void Jump()
    {
        if (jumpAmountRemaining > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpStrength);
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
    }

    private void Flip()
    {
        facingRight = !facingRight;
        sprite.transform.Rotate(0, 180, 0);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}

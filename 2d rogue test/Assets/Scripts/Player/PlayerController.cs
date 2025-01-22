
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Boolean Variables
    private bool facingRight = false;
    private bool isGrounded;
    private bool isTouchingWall;
    private bool isWallSliding;
    private bool isRolling;
    private bool canMove = true;
    private bool canJump = true;
    private bool canFlip = true;
    #endregion

    #region Integer Variables
    public int jumpAmount;
    private int jumpAmountRemaining;
    private int facingDirection = 1;
    #endregion

    #region Terrain Detection Variables
    public Transform groundCheck;
    public Transform wallCheck;
    public LayerMask Ground;
    #endregion

    #region Float Variables
    public float movespeed;
    public float jumpStrength;
    public float groundCheckRadius;
    public float wallCheckDistance;
    public float wallSlideSpeed;
    public float updateTime;
    public float rollTime;
    public float rollSpeed;
    public float rollCooldown;

    private float horizontal;
    private float ticker;
    private float lastRoll;
    private float rollTimeRemaining;
    #endregion

    #region Component Variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer sprite;
    public TMPro.TMP_Text varDisplay;
    public PlayerCombatController playerCombatController;
    public GameObject slideDust;
    #endregion



    #region Unity Callback Functions
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
        RollCheck();
        WallSlideCheck();
        CheckDirection();
        CheckForInput();
        UpdateAnimations();
        displayVariableOnScreen(rb.linearVelocityY);
    }

    private void FixedUpdate()
    {
        Move();
        CheckSurroundings();
    }

    private void UpdateAnimations()
    {
        anim.SetBool("isWalking", rb.linearVelocityX > 0.01f || rb.linearVelocityX < -0.01f);
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.linearVelocityY);
        anim.SetBool("isWallSliding", isWallSliding);
        anim.SetBool("isRolling", isRolling);
    }
    #endregion

    #region Action Functions
    private void Move()
    {
        if (canMove && !playerCombatController.isAttacking)
        {
            rb.linearVelocity = new Vector2(movespeed * horizontal, rb.linearVelocityY);
        }

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
        if (jumpAmountRemaining >= 0 && isWallSliding)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
        }

        else if (jumpAmountRemaining > 0 && canJump)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpStrength);
            jumpAmountRemaining--;
        }
    }

    private void Roll()
    {
        if (!isWallSliding && isGrounded)
        {
            isRolling = true;
            lastRoll = Time.time;
            rollTimeRemaining = rollTime;
        }
    }
    private void Flip()
    {   
        if (!isWallSliding && canFlip)
        {
            facingDirection *= -1;
            facingRight = !facingRight;
            sprite.transform.Rotate(0, 180, 0);
        }

    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, 0);
    }

    #endregion

    #region Check Functions
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

        if (Input.GetButtonDown("Fire3") && (Time.time >= (lastRoll + rollCooldown)))
        {
            Roll();
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

    private void RollCheck()
    {
        if (isRolling)
        {
            if (rollTimeRemaining > 0)
            {
                canMove = false;
                canFlip = false;
                rb.linearVelocity = new Vector2(rollSpeed * facingDirection, (rb.linearVelocityY)*0.97f);
                rollTimeRemaining -= Time.deltaTime;
            }

            if (rollTimeRemaining <= 0)
            {
                isRolling = false;
                canMove = true;
                canFlip = true;
            }
            else if (isTouchingWall)
            {
                isRolling = false;
                canMove = true;
                canFlip = true;
                lastRoll = 0;
            }
        }
        if (isWallSliding)
        {
            lastRoll = 0;
        }
    }

    private void WallSlideCheck()
    {
        isWallSliding = (isTouchingWall && !isGrounded && rb.linearVelocityY < 0);
    }
    #endregion

    #region Other Functions

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

    private void DisableFlip() => canFlip = false;

    private void EnableFlip() => canFlip = true;

    #endregion
}

using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    #region Variables
    private bool groundDetected, wallDetected;

    [SerializeField] private Transform groundCheck, wallCheck;

    [SerializeField] private LayerMask ground;

    [SerializeField] private float groundCheckDistance, wallCheckDistance, movespeed, maxHealth, knockbackDuration, updateTime;

    [SerializeField] private TMPro.TMP_Text varDisplay;

    private float currentHealth, knockbackStartTime, ticker, flipStartTime;

    private Rigidbody2D aliveRB;

    private GameObject alive;

    private int facingDirection, damageDirection;

    [SerializeField] private Vector2 knockbackSpeed;

    private Animator aliveAnim;

    private Vector2 movement;
    #endregion



    private void Start()
    {
        alive = transform.Find("Alive").gameObject;
        aliveRB = alive.GetComponent<Rigidbody2D>();
        aliveAnim = alive.GetComponent<Animator>();

        currentHealth = maxHealth;
        facingDirection = -1;
    }

    private enum State
    {
        Moving,
        Hurt,
        Dead
    }

    private State currentState;

    private void Update()
    {
        switch (currentState)
        {
            case State.Moving:
                UpdateMovingState(); break;
            case State.Hurt:
                UpdateHurtState(); break;
            case State.Dead:
                UpdateDeadState(); break;
        }

        displayVariableOnScreen(movement.x);
    }

    #region Moving State

    private void EnterMovingState()
    {

    }

    private void UpdateMovingState()
    {
        groundDetected = Physics2D.Raycast(groundCheck.position, transform.up, groundCheckDistance, ground);
        wallDetected = Physics2D.Raycast(wallCheck.position, transform.right, wallCheckDistance, ground);
        movement.Set(movespeed * facingDirection, aliveRB.linearVelocityY);
        aliveRB.linearVelocity = movement;

        if (!groundDetected || wallDetected)
        {
            Flip();
        }
    }

    private void ExitMovingState()
    {

    }
    #endregion

    #region Hurt State

    private void EnterHurtState()
    {
        knockbackStartTime = Time.time;
        movement.Set(knockbackSpeed.x * damageDirection, knockbackSpeed.y);
        aliveRB.linearVelocity = movement;
        aliveAnim.SetBool("Hurt", true);
    }

    private void UpdateHurtState()
    {
        if(Time.time > knockbackStartTime + knockbackDuration)
        {
            SwitchState(State.Moving);
        }
    }

    private void ExitHurtState()
    {
        aliveAnim.SetBool("Hurt", false);
    }
    #endregion

    #region Dead State

    private void EnterDeadState()
    {
        //spawn death particles
        Destroy(gameObject);
    }

    private void UpdateDeadState()
    {

    }

    private void ExitDeadState()
    {

    }
    #endregion

    #region Other Functions

    private void SwitchState(State state)
    {
        switch (currentState)
        {
            case State.Moving:
                ExitMovingState(); break;
            case State.Hurt:
                ExitHurtState(); break;
            case State.Dead:
                ExitDeadState(); break;
        }

        switch (state)
        {
            case State.Moving:
                EnterMovingState(); break;
            case State.Hurt:
                EnterHurtState(); break;
            case State.Dead:
                EnterDeadState(); break;
        }

        currentState = state;
    }

    private void Flip()
    {
        if (Time.time > flipStartTime + 0.5f)
        {
            flipStartTime = Time.time;
            facingDirection *= -1;
            alive.transform.Rotate(0.0f, 180.0f, 0.0f);
        }
    }

    private void Damage(float[] attackDetails)
    {
        currentHealth -= attackDetails[0];

        if (attackDetails[1] > alive.transform.position.x)
        {
            damageDirection = -1;
        }
        else
        {
            damageDirection = 1;
        }

        if (currentHealth > 0.0f)
        {
            SwitchState(State.Hurt);
        }
        else if (currentHealth <= 0.0f)
        {
            SwitchState(State.Dead);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(groundCheck.position, new Vector2(groundCheck.position.x, groundCheck.position.y - groundCheckDistance));
        Gizmos.DrawLine(wallCheck.position, new Vector2(wallCheck.position.x + wallCheckDistance, wallCheck.position.y));
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

    #endregion


}

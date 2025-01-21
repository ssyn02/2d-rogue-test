using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private int xInput;
    private bool isGrounded;
    private bool jumpInput;
    private bool jumpInputReleased;
    private bool isJumping;
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        isGrounded = player.GroundedCheck();
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.InputHandler.NormInputX;
        jumpInput = player.InputHandler.JumpInput;
        jumpInputReleased = player.InputHandler.JumpInputReleased;
        player.Anim.SetFloat("yVelocity", player.CurrentVelocity.y);

        CheckJumpRelease();

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            stateMachine.ChangeState(player.LandState);
        }

        else if (jumpInput && player.JumpState.CanJump())
        {
            stateMachine.ChangeState(player.JumpState);
        }

        else
        {
            player.FlipCheck(xInput);
            player.SetVelocityX(playerData.movementVelocity * xInput);
        }
    }


    private void CheckJumpRelease()
    {
        if (isJumping)
        {
            if (jumpInputReleased)
            {
                player.SetVelocityY(player.CurrentVelocity.y * playerData.jumpHeightMultiplier);
                isJumping = false;
            }

            else if (player.CurrentVelocity.y <= 0f)
            {
                isJumping = false;
            }
        }
    }

    public void SetJumping() => isJumping = true;

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}

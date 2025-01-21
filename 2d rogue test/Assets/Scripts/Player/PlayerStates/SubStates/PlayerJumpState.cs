using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    private int jumpAmountRemaining;

    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
        jumpAmountRemaining = playerData.jumpAmount;
    }

    public override void Enter()
    {
        base.Enter();

        player.SetVelocityY(playerData.jumpVelocity);
        isAbilityDone = true;
        jumpAmountRemaining--;
        player.InAirState.SetJumping();
    }
    
    public bool CanJump() => jumpAmountRemaining > 0;

    public void ResetJumps() => jumpAmountRemaining = playerData.jumpAmount;

    public void DecreaseJumps() => jumpAmountRemaining--;
}

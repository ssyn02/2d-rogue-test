using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public Vector2 RawMovementInput { get; private set; }
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }
    public bool JumpInputReleased {  get; private set; }
    
    [SerializeField] private float inputBufferTime;
    private float jumpInputStartTime;

    private void Update()
    {
        CheckJumpBuffer();
    }

    public void OnMoveInput(InputAction.CallbackContext context)
    {
        RawMovementInput = context.ReadValue<Vector2>();
        NormInputX = (int)(RawMovementInput * Vector2.right).normalized.x;
        NormInputY = (int)(RawMovementInput * Vector2.up).normalized.y;
    }

    public void OnJumpInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            JumpInput = true;
            JumpInputReleased = false;
            jumpInputStartTime = Time.time;
        }

        if (context.canceled)
        {
            JumpInputReleased = true;
        }
    }

    public void UseJumpInput() => JumpInput = false;

    private void CheckJumpBuffer()
    {
        if(Time.time >= (jumpInputStartTime + inputBufferTime))
        {
            JumpInput = false;
        }
    }
}

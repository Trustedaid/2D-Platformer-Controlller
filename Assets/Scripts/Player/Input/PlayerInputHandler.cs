using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    // Start is called before the first frame update


    public Vector2 RawMovementInput { get; private set; }
    
    public int NormInputX { get; private set; }
    public int NormInputY { get; private set; }
    public bool JumpInput { get; private set; }


    [SerializeField]
    private float inputHoldTime = 0.2f;

    private float jumpInputStartTime;
    private void Update()
    {
        CheckJumpInputHoldTime();
    }
    public PlayerInputHandler InputHandler { get; private set; }


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
            jumpInputStartTime = Time.time;
        }

    }
    public void UseJumpInput() => JumpInput = false; // same thing with   JumpInput = false;

    private void CheckJumpInputHoldTime()
    {
        if(Time.time >= jumpInputStartTime + inputHoldTime)
        {
            JumpInput = false;
        }
    }
}

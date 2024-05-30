using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(GroundChecker))]
[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    private Rigidbody rb;
    private GroundChecker groundChecker; // TODO: Make independant

    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpAmount = 1;
    public bool CanJump { get => GetCanJump();  }
    public bool CanMultiJump { get => GetCanMultiJump(); }
    public bool CanCoyoteJump { get => GetCanCoyoteJump(); }
    private float jumpCounter = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundChecker = GetComponent<GroundChecker>();
    }

    private void Update()
    {
        JumpCounterResetCheck();
    }

    private bool GetCanJump()
    {
        // Check if the player is on the ground or if they've reached the maximum jumps
        if (groundChecker.IsGrounded)
        {
            return true;
        }
        else
        {
            return false; 
        }
    }

    private bool GetCanMultiJump()
    {
        if (groundChecker.IsGrounded) { return false; }
        if (jumpAmount < 2) { return false; }
        if (jumpCounter < jumpAmount) { return false; }
        return true;
    }

    private bool GetCanCoyoteJump()
    {
        if (jumpCounter != 0) { return false; }
        return true;
    }

    private void OnLeftTrigger(InputValue value)
    {
        float input = value.Get<float>();
        if(input > 0.5) { 
            OnSouthButtonDown(value);
        }
    }

    private void OnLeftBumperDown(InputValue value)
    {
        OnSouthButtonDown(value);
    }

    private void OnSouthButtonDown(InputValue value)
    {
        if (GameOverBool.gameOver) return;
        if (CanJump)
        {
            AttemptJump();
        }
        else if (CanMultiJump)
        {
            AttemptDoubleJump();
        }
        else if(CanCoyoteJump)
        {
            ExecuteJump((Vector3.up + transform.forward).normalized);
        }
    }

    private void AttemptJump()
    {
        if (!groundChecker.IsGrounded)
            return;

        ExecuteJump(Vector3.up);
    }

    private void AttemptDoubleJump()
    {
        if (groundChecker.IsGrounded)
            return;

        Vector3 jumpDirection = (Vector3.up + rb.velocity.normalized).normalized;

        ExecuteJump(jumpDirection);
    }

    private void ExecuteJump(Vector3 jumpDirection)
    {
        // Increment jump counter
        jumpCounter++;

        // Perform jump
        rb.velocity = jumpDirection * jumpForce;
    }

    private void JumpCounterResetCheck()
    {
        if (jumpCounter == 0) { return; }
        if (rb.velocity.y > 0) { return; }

        if (groundChecker.IsGrounded) { 
            jumpCounter = 0; 
        }
    }
}
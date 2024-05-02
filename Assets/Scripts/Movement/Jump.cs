using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class Jump : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    private Rigidbody rb;
    private Gravity myGravity; // TODO: Make independant

    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float jumpAmount = 1;
    public bool CanJump { get => GetCanJump();  }
    public bool CanMultiJump { get => GetCanMultiJump(); }
    public bool CanCoyoteJump { get => GetCanCoyoteJump(); }
    private float jumpCounter = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myGravity = GetComponent<Gravity>();
    }

    private void Update()
    {
        JumpCounterResetCheck();
    }

    private bool GetCanJump()
    {
        // Check if the player is on the ground or if they've reached the maximum jumps
        if (myGravity != null && myGravity.IsGrounded)
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
        if (myGravity == null) { return false; }
        if (myGravity.IsGrounded) { return false; }
        if (jumpAmount < 2) { return false; }
        if (jumpCounter < jumpAmount) { return false; }
        return true;
    }

    private bool GetCanCoyoteJump()
    {
        if (myGravity == null) { return false; }
        if (jumpCounter != 0) { return false; }
        return true;
    }

    private void OnSouthButtonDown(InputValue value)
    {
        if (myGravity == null)
            return;

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
        if (myGravity == null || !myGravity.IsGrounded)
            return;

        ExecuteJump(Vector3.up);
    }

    private void AttemptDoubleJump()
    {
        if (myGravity == null || myGravity.IsGrounded)
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
        if (myGravity == null) { return; }
        if (jumpCounter == 0) { return; }
        if (rb.velocity.y > 0) { return; }

        if (myGravity.IsGrounded) { 
            jumpCounter = 0; 
        }
    }
    /*
    private void OnCollisionEnter(Collision collision)
    {
        if (myGravity != null && myGravity.IsGrounded)
        {
            // Reset jump counter when grounded
            jumpCounter = 0;
        }
    }*/
}
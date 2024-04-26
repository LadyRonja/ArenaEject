using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.LightAnchor;

public class Jump : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    private Rigidbody rb;
    private Gravity myGravity; // TODO: Make independant

    [SerializeField] private float jumpForce = 6f;
    private bool canJump = true;
    private bool canDoubleJump = true;
    private float jumpCounter = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myGravity = GetComponent<Gravity>();
    }

    private void Update()
    {
        // Check if the player is on the ground or if they've reached the maximum jumps
        if (myGravity != null && myGravity.IsGrounded || jumpCounter >= 2)
        {
            canJump = true;
            canDoubleJump = false;
        }
        else
        {
            canJump = false;
            canDoubleJump = true;
        }
    }

    private void OnSouthButtonDown(InputValue value)
    {
        if (myGravity == null)
            return;

        if (canJump)
        {
            AttemptJump();
        }
        else if (canDoubleJump)
        {
            AttemptDoubleJump();
        }
    }

    private void AttemptJump()
    {
        if (rb == null || myGravity == null || !myGravity.IsGrounded)
            return;

        ExecuteJump(Vector3.up);
    }

    private void AttemptDoubleJump()
    {
        if (rb == null || myGravity == null || myGravity.IsGrounded)
            return;

        Vector3 jumpDirection = Vector3.up + rb.velocity.normalized;

        ExecuteJump(jumpDirection);
    }

    private void ExecuteJump(Vector3 jumpDirection)
    {
        // Increment jump counter
        jumpCounter++;

        // Perform jump
        rb.velocity = jumpDirection * jumpForce;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (myGravity != null && myGravity.IsGrounded)
        {
            // Reset jump counter when grounded
            jumpCounter = 0;
        }
    }
}
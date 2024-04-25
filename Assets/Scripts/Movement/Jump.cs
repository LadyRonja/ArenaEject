using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Jump : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    private Rigidbody rb;
    private Gravity myGravity; // TODO: Make independant

    [SerializeField] private float jumpForce = 5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        myGravity = GetComponent<Gravity>();
    }

    private void OnSouthButtonDown(InputValue value)
    {
        if(myGravity == null) { return; }

        AttemptJump();
    }

    private bool AttemptJump()
    {
        if(rb == null) { return false; }
        if(myGravity == null) { return false; }
        if(!myGravity.IsGrounded) { return false; }


        ExecuteJump();
        return true;
    }

    private void ExecuteJump()
    {
        rb.AddForce(Vector3.up* jumpForce, ForceMode.Impulse);
    }
}

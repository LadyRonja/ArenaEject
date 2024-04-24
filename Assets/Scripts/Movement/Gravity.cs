using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private float gravityAmount = -9.82f;
    private Rigidbody rb;

    private bool IsGrounded { get => GetIsGrounded();  }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!IsGrounded)
        {
            rb.AddForce(Vector3.down * gravityAmount * Time.fixedDeltaTime);
        }
    }

    private bool GetIsGrounded()
    {
        float tempHeight = 1f;
        if (Physics.Raycast(transform.position, Vector3.down, tempHeight))
        {
            return true;
        }
        else
        {
            return false;
        }    
    }
}

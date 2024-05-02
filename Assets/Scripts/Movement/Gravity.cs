using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour
{
    [SerializeField] private float gravityUpAmount = -9.82f;
    [SerializeField] private float gravityDownAmount = -9.82f;
    private Rigidbody rb;

    public bool IsGrounded { get => GetIsGrounded();  }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }


    private void ApplyGravity()
    {
        if (!IsGrounded)
        {
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * gravityUpAmount * Time.fixedDeltaTime);
            }
            else
            {
                rb.AddForce(Vector3.down * gravityDownAmount * Time.fixedDeltaTime);
            }
        }
    }

    private bool GetIsGrounded()
    {
        float groundPlanckDist = 0.05f;
        if (Physics.Raycast(transform.position, Vector3.down, groundPlanckDist))
        {
            return true;
        }
        else
        {
            return false;
        }    
    }
}

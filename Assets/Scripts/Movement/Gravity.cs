using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundChecker))]
public class Gravity : MonoBehaviour
{
    [SerializeField] private float gravityUpAmount = -9.82f;
    [SerializeField] private float gravityDownAmount = -9.82f;
    [SerializeField] private float downSpeedMax = 5f;
    private Rigidbody rb;
    private GroundChecker groundChecker;

    //public bool IsGrounded { get => GetIsGrounded();  }

    private void Awake()
    {
        if(!TryGetComponent<Rigidbody>(out rb))
        {
            rb =gameObject.AddComponent<Rigidbody>();
        }
        groundChecker = GetComponent<GroundChecker>();
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (!groundChecker.IsGrounded)
        {
            if(rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * gravityUpAmount);
            }
            else
            {
                rb.AddForce(Vector3.down * Mathf.Abs(rb.velocity.y * gravityDownAmount));
                Vector3 clampVel = rb.velocity;
                clampVel.y = Mathf.Clamp(clampVel.y, -1f * downSpeedMax, 99999f);
                rb.velocity = clampVel;
            }
        }
    }
}

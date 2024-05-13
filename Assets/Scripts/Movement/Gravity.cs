using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GroundChecker))]
public class Gravity : MonoBehaviour
{
    [SerializeField] private float gravityUpAmount = -9.82f;
    [SerializeField] private float gravityDownAmount = -9.82f;
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
                Debug.Log("Heavy down!");
                rb.AddForce(Vector3.down * Mathf.Abs(rb.velocity.y * gravityDownAmount));
            }
        }
    }
}

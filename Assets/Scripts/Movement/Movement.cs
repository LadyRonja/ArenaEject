using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
	[SerializeField] private float maxSpeed = 18.0f;
    [SerializeField] private float acceleration = 20f;
	private Vector2 moveInput;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
	{
        MoveLogic(); // Physics simulation runs on fixed update, thus our physics based move logic should be ran here too
    }

    private void MoveLogic()
    {
        Vector3 perservedFallingVelocity = rb.velocity;

        // Acceleration
        if (moveInput != Vector2.zero)
        {
            Vector3 translatedMovement = new Vector3(moveInput.x, 0, moveInput.y);
            rb.velocity += acceleration * maxSpeed * Time.deltaTime * translatedMovement;
            if (rb.velocity.sqrMagnitude > maxSpeed * maxSpeed) rb.velocity = rb.velocity.normalized * maxSpeed;
        }
        else
        {
            // Deceleration
            rb.velocity -= acceleration * maxSpeed * Time.deltaTime * rb.velocity.normalized;
            if (rb.velocity.sqrMagnitude < 3f) rb.velocity = Vector3.zero;
        }

        Vector3 newVelocity = rb.velocity;
        newVelocity.y = perservedFallingVelocity.y;
        if (Mathf.Abs(newVelocity.y) < 0.1f) newVelocity.y = 0f;

        rb.velocity = newVelocity;
    }

    private void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        float x = moveInput.x;
        float z = moveInput.y;

        if (Mathf.Abs(x) < 0.3f) x = 0;
        if (Mathf.Abs(z) < 0.3f) z = 0;

        moveInput = new Vector2(x, z);
    }
}

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempControlls : MonoBehaviour
{
    Rigidbody rb;
    [SerializeField] float acceleration = 5f;
    [SerializeField] float maxSpeed = 10f;
    Vector2 moveInput = Vector2.zero;
    Vector2 aimInput = Vector2.zero;


    [SerializeField][Range(0.3f, 3.2f)] private float rotationSpeedRads = 1.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    private void FixedUpdate()
    {
        //MoveLogic();
        //AimLogic();
    }

    private void OnNorthButtonDown(InputValue value)
    {
        Debug.Log("North button!");
    }

    private void OnFire(InputValue value)
    {
        Debug.Log(value.Get<float>());
    }

    /*private void MoveLogic()
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
            if (rb.velocity.sqrMagnitude < 0.01) rb.velocity = Vector2.zero;
        }


        Vector3 newVelocity = rb.velocity;
        newVelocity.y = perservedFallingVelocity.y;
        rb.velocity = newVelocity;
    }

    private void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();

            float x = moveInput.x;
            float z = moveInput.y;

            if (Mathf.Abs(x) < 0.2f) x = 0;
            if (Mathf.Abs(z) < 0.2f) z = 0;

        moveInput = new Vector2(x, z);
    }*/
    /*
    private void AimLogic()
    {
        if (aimInput != Vector2.zero)
        {
            // Instant roation
            
            float angle = Mathf.Atan2(aimInput.x, aimInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);
            

            /*
            float angle = Mathf.Atan2(aimInput.x, aimInput.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeedRads);
        }
        else if (moveInput != Vector2.zero)
        {
            float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);

            /*float angle = Mathf.Atan2(moveInput.x, moveInput.y) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeedRads);
        }     
    }


    private void OnAiming(InputValue value)
    {
        aimInput = value.Get<Vector2>();
        float x = aimInput.x;
        float z = aimInput.y;

        if (Mathf.Abs(x) < 0.2f) x = 0;
        if (Mathf.Abs(z) < 0.2f) z = 0;

        aimInput = new Vector2(x, z);
    } */
}

using UnityEngine;
using UnityEngine.InputSystem;

public class Aiming : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    [SerializeField] public bool instantlyRotate = false;
    [SerializeField] [Range(5f, 30f)] private float rotationSpeedRads = 15;

    Vector2 aimInput = Vector2.zero; // Primnary Input
    Vector2 moveInput = Vector2.zero; // Secondary input


    private void Start()
    {
        aimInput = transform.forward;
    }

    private void FixedUpdate()
    {
        AimLogic();
    }

    private void OnAiming(InputValue value)
    {
        aimInput = value.Get<Vector2>();
        float x = aimInput.x;
        float z = aimInput.y;

        if (Mathf.Abs(x) < 0.2f) x = 0;
        if (Mathf.Abs(z) < 0.2f) z = 0;

        aimInput = new Vector2(x, z);
    }

    // Also look for move input for secondary priority
    private void OnMovement(InputValue value)
    {
        moveInput = value.Get<Vector2>();

        float x = moveInput.x;
        float z = moveInput.y;

        if (Mathf.Abs(x) < 0.2f) x = 0;
        if (Mathf.Abs(z) < 0.2f) z = 0;

        moveInput = new Vector2(x, z);
    }

    private void AimLogic()
    {
        if (StaticStats.gameOver) return;
        
        if (aimInput != Vector2.zero)
        {
            LookAt(aimInput);
        }
        else if (moveInput != Vector2.zero)
        {
            LookAt(moveInput);
        }
    }

    private void LookAt(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if (instantlyRotate)
        {
            transform.rotation = Quaternion.Euler(0, angle, 0);
        }
        else
        {
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeedRads);
        }
    }
}
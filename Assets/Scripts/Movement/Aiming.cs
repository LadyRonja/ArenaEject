using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Aiming : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;
    [HideInInspector] public int controllerIndex = 1;
    [SerializeField] [Range(0.3f, 3.2f)] private float rotationSpeedRads = 1;

    Vector3 targetDir = Vector3.zero;


    private void Start()
    {
        targetDir = transform.forward;
    }

    private void Update()
    {
        PlayerLook();
    }

    private void PlayerLook()
    {

        if (appropriatlySpawned)
        {
            float x = Input.GetAxisRaw($"P{controllerIndex}_Aim_Horizontal");
            float y = 0;
            float z = Input.GetAxisRaw($"P{controllerIndex}_Aim_Vertical") * -1f;

            if (Mathf.Abs(x) < 0.2f) x = 0;
            if (Mathf.Abs(z) < 0.2f) z = 0;

            targetDir = new Vector3(x, y, z);
        }
        else targetDir = new Vector3(Input.GetAxisRaw($"Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        if(targetDir.sqrMagnitude < 0.5f)
        {
            float x = Input.GetAxisRaw($"P{controllerIndex}_Horizontal_Duo");
            float y = 0;
            float z = Input.GetAxisRaw($"P{controllerIndex}_Vertical_Duo") * -1f;


            if (Mathf.Abs(x) > 0.2f || Mathf.Abs(z) > 0.2f)
            {
                targetDir = new Vector3(x, y, z);
            }

        }
        else
        {
            targetDir.Normalize();
        }

        if(targetDir != Vector3.zero)
        {
            // Instant roation
            /*
            float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, angle, 0);*/
            

            // Over time
            float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeedRads);
        }
    }
}
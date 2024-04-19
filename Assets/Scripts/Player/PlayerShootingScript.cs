using System.Collections.Generic;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootPoint;
    public float shootForce = 25f;
    public float fireRate = 0.5f;
    public float spawnDistance = 1.0f; // Adjust this distance as needed
    private float nextFireTime = 0f;

    [HideInInspector] public int controllerIndex = 1;

    public static Dictionary<int, int> shotsFiredPerPlayer = new Dictionary<int, int>();

    void Update()
    {
        if (Input.GetButtonDown($"Fire{controllerIndex}") && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + fireRate;
        }
    }

    void Shoot()
    {
        // Calculate spawn position in front of the player
        Vector3 spawnPosition = shootPoint.position + transform.forward * spawnDistance;
        spawnPosition.y += 1;

        // Instantiate the projectile at the calculated position
        GameObject projectile = Instantiate(projectilePrefab, spawnPosition, Quaternion.identity);

        if (shotsFiredPerPlayer.ContainsKey(controllerIndex))
        {
            shotsFiredPerPlayer[controllerIndex]++;
            Debug.Log(shotsFiredPerPlayer[controllerIndex]);
        }
        else
        {
            shotsFiredPerPlayer[controllerIndex] = 1;
        }
        
        // Get the Rigidbody component of the projectile
        Rigidbody rb = projectile.GetComponent<Rigidbody>();

        // Calculate the direction the projectile should travel
        Vector3 shootDirection = transform.forward;

        // Apply force to the projectile in the calculated direction
        if (rb != null)
        {
            rb.AddForce(shootDirection * shootForce, ForceMode.Impulse);
        }

        // Set the rotation of the projectile to match the shoot direction
        projectile.transform.rotation = Quaternion.LookRotation(shootDirection);
    }
}
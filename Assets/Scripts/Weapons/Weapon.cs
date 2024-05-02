using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Ammo ammoTypePrefab;
    public List<Transform> firePoints = new();
    public int ammoCount = 1;
    public float initialUpwardForce = 20f;
    [SerializeField] protected float fireRate = 0.5f;
    private float fireTimer = 0;
    private float spreadAngle = 20;
    [SerializeField] protected float knockbackForce = 10f;
    [SerializeField] protected bool weaponDeterminesAmmoSpeed = true;
    [SerializeField] protected bool fireAll = false;
    [SerializeField] protected float ammoSpeed = 3f;
    [SerializeField] protected Vector3 ammoDirOffSet = Vector3.zero;
    [SerializeField] protected float destroyDelay = 5f;
    protected int shotsFired = 0;

    public virtual bool TryShoot()
    {
        if (ammoCount <= 0) return false;
        if (ammoTypePrefab == null) return false;
        if (firePoints == null) return false;
        if (firePoints.Count == 0) return false;
        if (Time.time <= fireTimer) return false;
        
        Shoot();
        return true;
    }

    protected virtual void Shoot()
    {
        fireTimer = Time.time + fireRate;
        ammoCount--;
        shotsFired++;
        GameObject projectileObj;

        if (fireAll)
        {
            foreach (Transform firePoint in firePoints)
            {
                projectileObj = Instantiate(ammoTypePrefab, firePoint.position, Quaternion.identity).gameObject;

                Ammo projectileScr = projectileObj.GetComponent<Ammo>();
                Vector3 baseDirection = transform.forward;
                Vector3 randomDirection = Quaternion.Euler(Random.Range(-spreadAngle, spreadAngle), Random.Range(-spreadAngle, spreadAngle), 0) * baseDirection;
                randomDirection.y += initialUpwardForce;
                randomDirection.Normalize();
                projectileScr.moveDir = randomDirection;

                if (weaponDeterminesAmmoSpeed)
                {
                    projectileScr.moveSpeed = ammoSpeed;
                }
            }
        }
        else
        {
            projectileObj = Instantiate(ammoTypePrefab, firePoints[shotsFired % firePoints.Count].position, Quaternion.identity).gameObject;

            projectileObj.transform.forward = transform.forward;
            Ammo projectileScr = projectileObj.GetComponent<Ammo>();
            Vector3 projectileDir = projectileObj.transform.forward;
            ammoDirOffSet.Normalize();
            projectileDir.y += initialUpwardForce;
            projectileDir.Normalize();
            projectileScr.moveDir = projectileDir;

            if (weaponDeterminesAmmoSpeed)
            {
                projectileScr.moveSpeed = ammoSpeed;
            }
        }


        // 
        /*if (!projectileObj.GetComponent<Projectile>())
        {
            Rigidbody projectileRb = projectileObj.GetComponent<Rigidbody>();
            projectileRb.velocity = Vector3.zero;
            projectileRb.AddForce(Vector3.up * initialUpwardForce, ForceMode.Impulse);
        }*/

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<KnockBackHandler>(out KnockBackHandler hit))
        {
            Rigidbody rb = gameObject.GetComponent<Rigidbody>();

            if(rb != null) 
            {
                Vector3 dir = rb.velocity.normalized;
                dir.y = 0;
                Vector3 velocityCheck = rb.velocity;
                velocityCheck.y = 0;
                if(velocityCheck.sqrMagnitude > 1f) { hit.GetKnockedBack(dir, knockbackForce); }
                
            }
            Destroy(gameObject);
        }
        DestroyAfterDelay();
    }

    protected virtual void DestroyAfterDelay()
    {
        Destroy(gameObject, destroyDelay);
    }
}

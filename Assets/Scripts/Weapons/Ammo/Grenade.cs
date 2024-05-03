using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : Ammo
{
    [SerializeField] protected bool explodeOnImpact = false;
    [SerializeField] protected float explodeTime = 1.5f;
    [SerializeField] protected float explosionDelay = 10f;
    [SerializeField] protected float proximityDistance = 6f;
    [SerializeField] protected GameObject explosionPrefab;
    public Collider triggerCollider;
    public LayerMask freezingGroundLayer;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = moveDir * moveSpeed;
    }

    private void Update()
    {
        if (rb.isKinematic)
        {
            Collider[] colliders = Physics.OverlapSphere(transform.position, proximityDistance);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag("Player"))
                {
                    float distance = Vector3.Distance(transform.position, collider.transform.position);
                    if (distance <= proximityDistance)
                    {
                        Explode();
                        break; 
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<KnockBackHandler>(out KnockBackHandler hit))
        {   
            // apply knockback if moving fast enough
            if (rb.velocity.sqrMagnitude > 2f)
            {
                Vector3 dir = rb.velocity.normalized;
                dir.y = 0;
                hit.GetKnockedBack(dir, knockbackForce);
            }
        }
        else
        {
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            transform.up = other.contacts[0].normal;

            DelayExplosion();
        }

        if (!explodeOnImpact) return;

        if (other.gameObject.CompareTag("KillPlane") || other.gameObject.CompareTag("Ground")) return;
    }

    private void DelayExplosion()
    {
        this.Invoke(nameof(Explode), explosionDelay);
    }

    private void Explode()
    {
        this.CancelInvoke();

        if(explosionPrefab == null)
        {
            Destroy(gameObject);
            Debug.Log("NOEXPLOSION");
            return;
        }

        GameObject explosionObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, proximityDistance);
    }
}


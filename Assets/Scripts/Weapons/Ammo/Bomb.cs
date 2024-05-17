using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bomb : Ammo
{
    [SerializeField] protected float explodeTime = 1.5f;
    [SerializeField] protected GameObject explosionPrefab;
    [SerializeField] protected bool explodeOnImpact;

    protected override void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = moveDir * moveSpeed;

        this.Invoke(nameof(Explode), explodeTime);
    }

    private void FixedUpdate()
    {
        if (rb.velocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(rb.velocity);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent<KnockBackHandler>(out KnockBackHandler hit))
        {
            if (other.gameObject.TryGetComponent<PlayerStats>(out PlayerStats hitPlayer))
            {
                if (hitPlayer.playerIndex == ownerIndex)
                {
                    return;
                }
            }
            Explode();
        }

        if(other.gameObject.TryGetComponent<BoxCollider>(out BoxCollider collider))
        {
            Explode();
        }

        if (!explodeOnImpact) return;
    }

    private void Explode()
    {
        this.CancelInvoke();

        if (explosionPrefab == null)
        {
            Destroy(gameObject);
            return;
        }

        GameObject explosionObj = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}

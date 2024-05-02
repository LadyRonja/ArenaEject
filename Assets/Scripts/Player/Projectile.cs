using System.Collections;
using UnityEngine;

public class Projectile : Ammo
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<KnockBackHandler>(out KnockBackHandler hit))
        {
            Vector3 dir = rb.velocity.normalized;
            dir.y = 0;
            hit.GetKnockedBack(dir, knockbackForce);
        }
        if (other.gameObject.CompareTag("Projectile"))
        {
            return;
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
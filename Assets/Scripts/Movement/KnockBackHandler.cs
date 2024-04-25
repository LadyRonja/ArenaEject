using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class KnockBackHandler : MonoBehaviour
{
    private Rigidbody rb;

    public float frictionCoefficient = 10f;
    public float frictionDelay = 0.3f;
    private bool knockedBack = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /*void FixedUpdate()
    {
        if (!knockedBack) return; 

        if(rb != null)
        {
            if (rb.velocity.sqrMagnitude > 0)
            {
                Deaccelerate();
            }
        }
    }*/

    public void Deaccelerate()
    {
        rb.velocity *= 1f/frictionCoefficient;
        if (rb.velocity.sqrMagnitude < 1f) // Check if velocity is close to zero
        {
            rb.velocity = Vector3.zero; // Ensure velocity is exactly zero
            this.CancelInvoke();
            if (TryGetComponent<Movement>(out Movement myMovement))
            {
                myMovement.enabled = true;
                knockedBack = false;
            }
        }
    }

    public void GetKnockedBack(Vector3 dir, float force)
    {
        if(TryGetComponent<Movement>(out Movement myMovement))
        {
            myMovement.enabled = false;
        }

        this.CancelInvoke();
        rb.velocity = Vector3.zero;
        knockedBack = true;
        rb.AddForce(dir * force, ForceMode.Impulse);
        InvokeRepeating(nameof(Deaccelerate), frictionDelay, Time.fixedDeltaTime);
    }
}

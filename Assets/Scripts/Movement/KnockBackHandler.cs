using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void KnockbackRecieved();

[RequireComponent(typeof(Rigidbody))]
public class KnockBackHandler : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private bool usingCumulativeKnockback = true;
    private float recievedKnockbackRaw = 0;
    public float recievedKnockbackDisplay { get => Mathf.Ceil(Mathf.Max(0, Mathf.Min(recievedKnockbackRaw, 999))); }
    public bool KnockedBack { get => knockedBack; }

    public float frictionCoefficient = 10f;
    public float frictionDelay = 0.3f;
    private bool knockedBack = false;

    public KnockbackRecieved OnKnockbackRecieved;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

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
        dir.Normalize();

        recievedKnockbackRaw += force;
        float forceToApply = force;

        if (usingCumulativeKnockback){
            forceToApply *= Mathf.Max(1f + (recievedKnockbackRaw * 0.1f), 1f);
        }

        rb.velocity = Vector3.zero;
        rb.AddForce(dir * forceToApply, ForceMode.Impulse);

        OnKnockbackRecieved?.Invoke();

        knockedBack = true;
        InvokeRepeating(nameof(Deaccelerate), frictionDelay, Time.fixedDeltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void KnockbackRecieved();
public delegate void KnockbackComplete();

[RequireComponent(typeof(Rigidbody))]
public class KnockBackHandler : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float knockbackMultiplicator = 0.5f;
    [SerializeField] private bool usingCumulativeKnockback = true;
    private float recievedKnockbackRaw = 0;
    public float recievedKnockbackDisplay { get => Mathf.Ceil(Mathf.Max(0, Mathf.Min(recievedKnockbackRaw, 999))); }
    public bool KnockedBack { get => knockedBack; }

    public float frictionCoefficient = 10f;
    public float frictionDelay = 0.3f;
    private bool knockedBack = false;

    public KnockbackRecieved OnKnockbackRecieved;
    public KnockbackComplete OnKnockbackComplete;

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
                myMovement.isKnocked = false;
                knockedBack = false;
            }

            if (TryGetComponent<Aiming>(out Aiming myAim))
            {
                myAim.enabled = true;
            }

            Debug.Log("knockback complete!");
            OnKnockbackComplete?.Invoke();
        }
    }

    public void GetKnockedBack(Vector3 dir, float force, bool locksPlayerForDuration)
    {
        if (TryGetComponent<PlayerStats>(out PlayerStats myPlayerStats))
        {
            if (!myPlayerStats.alive)
            {
                myPlayerStats.finalKnockbackDisplay = recievedKnockbackDisplay;
            }
        }
        
        // Halving all force recieved for faster platesting
        force *= knockbackMultiplicator;

        if(locksPlayerForDuration)
        {
            if (TryGetComponent<Movement>(out Movement myMovement))
            {
                myMovement.enabled = false;   
                myMovement.isKnocked = true;
            }

            if(TryGetComponent<Aiming>(out Aiming myAim))
            {
                myAim.enabled = false;
            }
        }
        else
        {
            if (TryGetComponent<Movement>(out Movement myMovement))
            {
                myMovement.isKnocked = true;
            }
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

        if (locksPlayerForDuration)
        {
            transform.forward = dir * -1f;
        }

        OnKnockbackRecieved?.Invoke();

        knockedBack = true;
        InvokeRepeating(nameof(Deaccelerate), frictionDelay, Time.fixedDeltaTime);
    }
}

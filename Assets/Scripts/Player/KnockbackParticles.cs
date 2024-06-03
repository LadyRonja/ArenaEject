using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class KnockbackParticles : MonoBehaviour
{
    private protected KnockBackHandler playerKnockback;
    private VisualEffect vfx;
    private protected bool canPlay = true;

    private bool CanPlayParticles { get { return ((ComponentsVerified() && canPlay) == true); } }

    void Start()
    {
        vfx = GetComponentInChildren<VisualEffect>();
        SetUp();
    }

    void SetUp()
    {
        if (!TryGetComponent<KnockBackHandler>(out playerKnockback))
        {
            Debug.LogError("KnockbackParticles Cannot find KnockBackHandler component");
        }
        else
        {
            if(CanPlayParticles)
            {
                playerKnockback.OnKnockbackRecieved += CauseKnockbackParticles;
            }
        }
    }

    void Update()
    {
        if (!TryGetComponent<KnockBackHandler>(out playerKnockback))
        {
            Debug.LogError("KnockbackParticles Cannot find KnockBackHandler component");
        }
        else
        {
            if (CanPlayParticles)
            {
                playerKnockback.OnKnockbackRecieved += CauseKnockbackParticles;
            }
        }
    }

    private void OnDestroy()
    {
        if (ComponentsVerified())
        {
            playerKnockback.OnKnockbackRecieved -= CauseKnockbackParticles;
        }
    }

    private void CauseKnockbackParticles()
    {
        if (!CanPlayParticles) { return; }
        canPlay = false;
        vfx.Play();
        canPlay = true;
    }

    private bool ComponentsVerified()
    {
        if (playerKnockback == null) { return false; }

        return true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootToEscape : MonoBehaviour
{
    private void OnCollisionEnter(Collision other)
    {

        if(!other.gameObject.TryGetComponent<Ammo>(out Ammo hitBy))
        {
            return;
        }

        if(TryGetComponent<BoxCollider>(out BoxCollider myCollider))
        {
            myCollider.enabled = false;
        }
        this.gameObject.AddComponent<Rigidbody>();

        Destroy(this.gameObject, 5f);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (!other.gameObject.TryGetComponent<Ammo>(out Ammo hitBy))
        {
            return;
        }

        if (TryGetComponent<BoxCollider>(out BoxCollider myCollider))
        {
            myCollider.enabled = false;
        }
        this.gameObject.AddComponent<Rigidbody>();

        Destroy(this.gameObject, 5f);
    }
}

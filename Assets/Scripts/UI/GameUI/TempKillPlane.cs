using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKillPlane : MonoBehaviour
{
    bool activated = false;

    private void Start()
    {
        Invoke(nameof(Activate), 2f);
    }

    private void Activate()
    {
        activated = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!activated)
        {
            return;
        }
        if(other.gameObject.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            EndGameManager.Instance.PlayerDied(player);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempKillPlane : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            EndGameManager.Instance.PlayerDied(player);
        }
    }
}

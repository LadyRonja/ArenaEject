using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReadyRegion : MonoBehaviour
{
    List<PlayerStats> playersInRegion = new();

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            playersInRegion.Add(player);
            CheckPlayCondition();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            playersInRegion.Remove(player);
        }
    }

    private void CheckPlayCondition()
    {
        if(playersInRegion.Count < 2) { return; }

        PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
        if(allPlayers.Length == allPlayers.Length ) { 
            StartScreenManager.Instance.StartGame();
        }
    }
}

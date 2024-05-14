using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReadyRegion : MonoBehaviour
{
    private bool startingGame = false;
    List<ReadyBehaivor> playersInRegion = new();

    private void Update()
    {
        CheckPlayCondition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ReadyBehaivor>(out ReadyBehaivor player))
        {
            playersInRegion.Add(player);
            player.inReadyArea = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ReadyBehaivor>(out ReadyBehaivor player))
        {
            player.inReadyArea = false;
            player.isReady = false;
            player.HideReady();
            playersInRegion.Remove(player);
        }
    }

    private void CheckPlayCondition()
    {
        if(playersInRegion.Count < 2) { return; }
        if(startingGame) { return; }

        ReadyBehaivor[] allPlayers = (ReadyBehaivor[])FindObjectsOfType(typeof(ReadyBehaivor));
        if(allPlayers.Length != allPlayers.Length ) { return; }

        if(playersInRegion.All(p => p.isReady))
        {
            StartCoroutine(StartGame());
        }   
    }

    private IEnumerator StartGame()
    {
        startingGame = true;
        StartScreenManager.Instance.StartGame();
        yield return null;
    }
}

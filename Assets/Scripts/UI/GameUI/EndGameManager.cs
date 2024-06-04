using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager instance;
    public static EndGameManager Instance { get { return GetInstance(); } }

    List<PlayerStats> deadPlayers = new();


    public void PlayerDied(PlayerStats player)
    {
        if (!deadPlayers.Contains(player))
        {
            player.lives = 0;
            deadPlayers.Add(player);
        }
        else
        {
            Debug.LogError("Player died multiple times");
            return;
        }
        player.gameObject.SetActive(false);
        List<PlayerStats> allPLayers = FindAllPlayers(true);
        foreach (PlayerStats player in allPLayers)
        {

        }
        
    }

    List<PlayerStats> FindAllPlayers(bool includeInactive)
    {
        // Check how many players are alive still alive
        PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats), includeInactive);
        List<PlayerStats> players = new();
        foreach (PlayerStats p in allPlayers)
        {
            players.Add(p);
        }

        return players;
    }

    private static EndGameManager GetInstance()
    { 
        if(instance != null)
        {
            return instance;
        }

        EndGameManager newInstance = new GameObject("End Game Manager").AddComponent<EndGameManager>();
        instance= newInstance;
        return instance;
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager instance;
    public static EndGameManager Instance { get { return GetInstance(); } }

    List<PlayerStats> deadPlayers = new();
    bool gameIsOver = false;
    [SerializeField] private GameObject endGameCanvasPrefab;
    [SerializeField] private TempPLayerEndPotrait endGamePotraitProfilePrefab;

    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance= this;
            gameIsOver = false;
        }
        else
        {
            Destroy(gameObject);
        }
    }

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
        List<PlayerStats> alivePLayers = FindAllPlayers(true);
        List<PlayerStats> removeFromAlive = new();
        foreach (PlayerStats p in alivePLayers)
        {
            if(!p.alive)
            {
                removeFromAlive.Add(p);
            }
        }
        foreach (PlayerStats p in removeFromAlive)
        {
            alivePLayers.Remove(p);
        }

        if(alivePLayers.Count == 1)
        {
            EndGame(alivePLayers[0]);
        }     
        else if(alivePLayers.Count == 0) {
            EndGame(player);
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

    private void EndGame(PlayerStats winner)
    {
        if (gameIsOver) { return; }

        gameIsOver = true;

        if(endGameCanvasPrefab == null )
        {
            FailSafe("endGameCanvasPrefab missing! Instantly going to next level");
            return;
        }

        if(endGamePotraitProfilePrefab == null)
        {
            FailSafe("endGamePotraitProfilePrefab missing! Instantly going to next level");
            return;
        }

        GameObject gameOverScreen = Instantiate(endGameCanvasPrefab);
        Transform potraitParent = this.transform;
        try
        {
            potraitParent = gameOverScreen.GetComponent<TempEndGameCanvas>().playerGridRegion;
        }
        catch
        {
            FailSafe("Unable to find potraitParent");
            return;
        }

        List<PlayerStats> allPlayers = FindAllPlayers(true);

        foreach (PlayerStats p in allPlayers)
        {
            TempPLayerEndPotrait potrait = Instantiate(endGamePotraitProfilePrefab, potraitParent);
            potrait.background.color = p.colors[p.playerIndex];
            potrait.picture.color = p.colors[p.playerIndex];
            Debug.Log("Picture is changing color temporarily, this is only until the picture is functionally accessible");
            if(p == winner)
            {
                potrait.winnerText.text = "Winner!";
            }
            else
            {
                potrait.winnerText.text = "";
            }

        }

        //Invoke(nameof(GoToNextScene), 5f);

        void FailSafe(string debugMsg)
        {
            Debug.Log(debugMsg);
            int randomLevel = Random.Range(1, 3);
            SceneHandler.Instance.GoToScene(randomLevel);
        }
    }

    public void GoToNextScene()
    {
        int randomLevel = Random.Range(1, 3);
        SceneHandler.Instance.GoToScene(randomLevel);
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

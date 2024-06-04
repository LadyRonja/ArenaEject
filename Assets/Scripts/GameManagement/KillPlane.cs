using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Coffee.UIExtensions;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class KillPlane : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new();
    private List<PlayerStats> players = new();
    private static List<PlayerStats> deadPlayers = new();
    private List<PlayerPotrait> playerPotraits = new List<PlayerPotrait>();
    
    private Dictionary<int, int> timeAliveTracking = new Dictionary<int, int>();
    private static bool isTimeTrackingStarted = false;
    
    [SerializeField] PlayerPotrait endGamePlayerPotraitPrefab;

    [Header("Temp")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject endGameLoading;
    [SerializeField] private TMP_Text endGameTitle;
    
    [SerializeField] private GameObject winnerAvatar;
    [SerializeField] private GameObject winnerStats;
    [SerializeField] private TMP_Text timeAliveWinner;
    [SerializeField] private TMP_Text shotsFiredWinner;

    [SerializeField] private GameObject secondAvatar;
    [SerializeField] private GameObject secondStats;
    [SerializeField] private TMP_Text timeAliveSecond;
    [SerializeField] private TMP_Text shotsFiredSecond;

    [SerializeField] private GameObject thirdAvatar;
    [SerializeField] private GameObject thirdStats;
    [SerializeField] private TMP_Text timeAliveThird;
    [SerializeField] private TMP_Text shotsFiredThird;

    [SerializeField] private GameObject fourthAvatar;
    [SerializeField] private GameObject fourthStats;
    [SerializeField] private TMP_Text timeAliveFourth;
    [SerializeField] private TMP_Text shotsFiredFourth;

    [SerializeField] private List<AudioClip> playerDeathSounds;

    [SerializeField] private int levelLoadTime = 3;

    [SerializeField] private Button menu;
    [SerializeField] private Button next;

    [SerializeField] protected float deathDelay = 3f;

    // TEMP
    private void Awake()
    {
        Time.timeScale = 1.0f;
        deadPlayers = new();
    }

    private void Start()
    {
        timeAliveTracking = new();
        if (!isTimeTrackingStarted)
        {
            isTimeTrackingStarted = true;
            StartCoroutine(IncrementTimeAlive());
        }
    }

    private IEnumerator IncrementTimeAlive()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);

            PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
            foreach (PlayerStats player in allPlayers)
            {
                if (player.alive)
                {
                    player.timeAlive++;
                    timeAliveTracking[player.playerIndex] = player.timeAlive;
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the game is already over
        if (StaticStats.gameOver) return;

        // Did a player collide?
        if(other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            // If this player hasn't collided before, save a reference to them.
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            
            // Subtract a life from the player, clamp to 0
            player.lives--;
            if (player.lives <= 0)
            {
                player.lives = 0;
            }

            // Play a deathsound effect
            AudioHandler.PlayRandomEffectFromList(playerDeathSounds);

            // If the player is now dead
            // Check if there is a refence to the player in deadPlayers list
            // If not, set how much total knockback they've recieved
            // Then add them to the list of deadPlayers
            if(!player.alive)
            {
                if (!deadPlayers.Contains(player))
                {
                    if(player.TryGetComponent<KnockBackHandler>(out KnockBackHandler knockbackHandler))
                    {
                        player.finalKnockbackDisplay = knockbackHandler.recievedKnockbackDisplay;
                    }
                    
                    deadPlayers.Add(player);
                }
            }

            // If the player is still alive,
            // Update the camera tracking
            // And respawn the player after a delay
            if (player.alive)
            {
                List<Transform> activePlayers = FindAllPlayerTransforms(false);
                foreach (Transform activePlayer in activePlayers)
                {
                    if (activePlayer == player.transform)
                    {
                        activePlayers.Remove(activePlayer);
                    }
                }
                CameraController.Instance.StartTrackingObjects(activePlayers);
                RespawnAfterDelay();
            }
            // If the player is dead
            else 
            {
                // Check how many players are alive still alive
                List<PlayerStats> allPlayers = FindAllPlayers(false);
                List<PlayerStats> alivePlayers = new(); 
                foreach (PlayerStats p in allPlayers)
                {
                    if (p.alive) alivePlayers.Add(p);
                }

                // Update camera tracking with only alive players
                List<Transform> alivePlayerTransforms = new();
                foreach (PlayerStats p in alivePlayers)
                {
                    alivePlayerTransforms.Add(p.transform);
                }
                CameraController.Instance.StartTrackingObjects(alivePlayerTransforms);

                // If fewer than 1 player is alive
                // The game is a tie
                // This is probably not possible, at the very least it's highly unlikely
                // If it does happen I'll just let this player become the winner.
                if (alivePlayers.Count < 1)
                {
                    StaticStats.gameOver = true;
                    Debug.LogError("No implementation for a TIED game, picking winner");

                    // Ending the game making 
                    EndGame(player);
                }
                // If there is only one player left alive
                // That is the winner
                // End the game
                else if (alivePlayers.Count == 1)
                {
                    // Winner
                    EndGame(alivePlayers[0]);
                }

                // If the player had a weapon user script
                // Save how many total shots they had fired during the game
                if(player.TryGetComponent<WeaponUser>(out WeaponUser dyingUser))
                {
                    try
                    {
                        PlayerShooting.shotsFiredPerPlayer.Add(player.playerIndex, dyingUser.shotsFired);
                    }
                    catch 
                    {
                        PlayerShooting.shotsFiredPerPlayer[player.playerIndex] = dyingUser.shotsFired;
                    }
                }

                // Disable the player
                player.gameObject.SetActive(false);
            }
        }
        // If it wasn't a player that was encountered, destory it after a short delay
        else
        {
            //Destroy(other.gameObject, 3f);
        }
    }

    /// <summary>
    /// Find all players in the scene
    /// </summary>
    /// <param name="includeInactive">Determine if the players can be found while inactive in the hierarchy</param>
    /// <returns></returns>
    List<PlayerStats> FindAllPlayers(bool includeInactive)
    {
        // Check how many players are alive still alive
        PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats), includeInactive);
        List<PlayerStats> players = new();
        foreach (PlayerStats p in allPlayers)
        {
            players.Add(p);
        }


        #region Unoptomized
        /*
        // Find all game objects in the scene
        Transform[] allObjects = FindObjectsOfType<Transform>(true);

        // Filter the objects to those named "Player" and convert to a list
        List<Transform> playerObjects = allObjects.Where(obj => obj.name == "Player").ToList();
        */
        #endregion

        return players;
    }

    /// <summary>
    /// Find all player transforms in the scene
    /// </summary>
    /// <param name="includeInactive">Determine if the players can be found while inactive in the hierarchy</param>
    /// <returns></returns>
    List<Transform> FindAllPlayerTransforms(bool includeInactive)
    {
        List<PlayerStats> playerStats = FindAllPlayers(includeInactive);

        List<Transform> transforms = new();
        foreach (PlayerStats p in playerStats)
        {
            transforms.Add(p.transform);
        }

        return transforms;
    }

    private void RespawnAfterDelay()
    {
        this.Invoke(nameof(RespawnPlayer), 5f); // Deathdelay seems to not be working??
    }

    private void RespawnPlayer()
    {
        // Respawn
        this.CancelInvoke();

        PlayerStats player = players[0];

        Vector3 respawnPos = Vector3.zero;
        try
        {
            int rand = Random.Range(0, spawnPoints.Count);
            respawnPos = spawnPoints[rand].position;
        }
        catch
        {
            Debug.LogError("No Respawn Points Listed, spawns player at 0,0,0");
        }

        player.transform.position = respawnPos;
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null) rb.velocity = Vector3.zero;
    }

    private void EndGame(PlayerStats winner)
    {
        // Early exit if reference is missing
        if (endGameCanvas == null) {
            Debug.LogError("Missing endGameCanvas, instantly going to start screen");
            SceneHandler.Instance.GoToScene(Paths.START_SCENE_NAME);
            return; 
        }
        // Set the game to be over
        StaticStats.gameOver = true;

        // If the winner isn't in the deadPlayers list
        // Try and get their total knockback recieved
        // Then add them to the list
        if (!deadPlayers.Contains(winner))
        {
            if(winner.TryGetComponent<KnockBackHandler>(out KnockBackHandler knockbackHandler))
            {
                winner.finalKnockbackDisplay = knockbackHandler.recievedKnockbackDisplay;
            }
            
            deadPlayers.Add(winner);
        }
        
        // If this player has won a game before
        // Up their win total
        // Else
        // Give them a win total of 1
        if (StaticStats.playerWins.ContainsKey(winner.playerIndex))
        {
            StaticStats.playerWins[winner.playerIndex]++;
        }
        else
        {
            StaticStats.playerWins.Add(winner.playerIndex, 1);
        }
        
        // Set all desired parent objects
        GameObject[] avatars = new GameObject[] { winnerAvatar, secondAvatar, thirdAvatar, fourthAvatar };
        
        // Clear old potraits
        playerPotraits.Clear();
        
        // For each dead player, update their potrait stats according to all fetched data
        for (int i = 0; i < deadPlayers.Count; i++)
        {
            PlayerStats playerStats = deadPlayers[i];

            PlayerPotrait playerPotraitInstance = Instantiate(endGamePlayerPotraitPrefab, avatars[deadPlayers.Count - 1 - i].transform);

            if(deadPlayers[i].TryGetComponent<PlayerUIHandler>(out PlayerUIHandler uiHandler))
            {
                uiHandler.UpdateEndGameUI();
            }
            
            playerPotraitInstance.background.color = playerStats.colors[playerStats.playerIndex];
            playerPotraitInstance.playerPotrait.sprite = playerStats.endGamePlayerSprites[playerStats.playerIndex];
            playerPotraitInstance.playerWins.text = StaticStats.playerWins.ContainsKey(playerStats.playerIndex) ? $"{StaticStats.playerWins[playerStats.playerIndex]} Wins" : "0 Wins";
            playerPotraitInstance.damagePercentage.text = $"{playerStats.finalKnockbackDisplay}%";
            playerPotraitInstance.transform.localScale = Vector3.zero;
            playerPotraitInstance.gameObject.SetActive(true);
            playerPotraits.Add(playerPotraitInstance);
        }

        // Go to display the end game panel
        ShowEndGamePanel(winner.playerIndex);
    }

    private void EndGameTie()
    {
        if(endGameCanvas == null) return;
        StaticStats.gameOver = true;

        ShowEndGamePanel(0);
    }

    private void ShowEndGamePanel(int winnerIndex)
    {
        foreach (PlayerUIHandler playerUIHandler in GameStartManager.playerUIHandlers)
        {
            playerUIHandler.HidePlayerPotraits();
        }
        
        List<GameObject> avatars = new List<GameObject> { winnerAvatar, secondAvatar, thirdAvatar, fourthAvatar };
        List<GameObject> statsContainers = new List<GameObject> { winnerStats, secondStats, thirdStats, fourthStats };
        List<TMP_Text> timeAliveTexts = new List<TMP_Text> { timeAliveWinner, timeAliveSecond, timeAliveThird, timeAliveFourth };
        List<TMP_Text> shotsFiredTexts = new List<TMP_Text> { shotsFiredWinner, shotsFiredSecond, shotsFiredThird, shotsFiredFourth };

        endGameCanvas.SetActive(true);
        endGameTitle.text = "Player " + (winnerIndex + 1) + " Wins!";
        endGameTitle.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);
        
        foreach (var text in timeAliveTexts)
            text.gameObject.SetActive(false);

        foreach (var text in shotsFiredTexts)
            text.gameObject.SetActive(false);
        
        foreach (var statsContainer in statsContainers)
            statsContainer.SetActive(false);

        foreach (var avatar in avatars)
            avatar.SetActive(false);

        DOVirtual.DelayedCall(1f, () =>
        {
            endGamePanel.transform.DOScale(1, 1f).SetEase(Ease.OutQuart);
        });
        
        int playerCount = Math.Min(deadPlayers.Count, timeAliveTexts.Count);
        playerCount = Math.Min(playerCount, shotsFiredTexts.Count);
        playerCount = Math.Min(playerCount, playerPotraits.Count);
        
        for (int i = 0; i < playerCount; i++)
        {
            timeAliveTexts[i].gameObject.SetActive(true);
            shotsFiredTexts[i].gameObject.SetActive(true);
            statsContainers[i].SetActive(true);
            avatars[i].SetActive(true);
        }

        List<PlayerPotrait> reversedPlayerPotraits = new List<PlayerPotrait>(playerPotraits);
        reversedPlayerPotraits.Reverse();

        for (int i = 0; i < playerCount; i++)
        {
            int index = i;
            PlayerStats playerStats = deadPlayers[deadPlayers.Count - 1 - index];
            PlayerPotrait playerPotrait = reversedPlayerPotraits[i];
            
            float delay = i == 0 ? 1.5f : 3.5f + (0.1f * (index - 1));

            DOVirtual.DelayedCall(delay + 0.1f, () =>
            {
                var tween = playerPotrait.transform.DOScale(0.6f, 0.5f).SetEase(Ease.InOutQuint);
                if (index == 0)
                {
                    tween.OnComplete(() =>
                    {
                        Transform bgTransform = playerPotrait.transform.Find("BG");
                        if (bgTransform != null)
                        {
                            UIShiny shineEffect = bgTransform.GetComponent<UIShiny>();
                            shineEffect.Play();
                        }
                        
                        playerPotrait.playerWins.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
                    });
                }

                else
                {
                    tween.OnComplete(() =>
                    {
                        playerPotrait.playerWins.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
                    });
                }
            });

            DOVirtual.DelayedCall(delay + 1.2f, () =>
            {
                timeAliveTexts[index].text = $"Survived {playerStats.timeAlive} s";
                timeAliveTexts[index].transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
            });
            
            DOVirtual.DelayedCall(delay + 1.4f, () =>
            {
                int shotsFired = PlayerShooting.shotsFiredPerPlayer.ContainsKey(playerStats.playerIndex) ? PlayerShooting.shotsFiredPerPlayer[playerStats.playerIndex] : 0;
                shotsFiredTexts[index].text = $"Fired {shotsFired} shots";
                shotsFiredTexts[index].transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
            });
        }

        DOVirtual.DelayedCall(6f, () =>
        {
            menu.interactable = true;
            next.interactable = true;
            
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            eventSystem.SetSelectedGameObject(next.gameObject);
            
            menu.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
            next.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
        });
    }
    
    public void OnMenuButtonPress()
    {
        //SceneManager.LoadScene(0);
        SceneHandler.Instance.GoToScene(Paths.START_SCENE_NAME);
    }
    
    public void OnNextButtonPress()
    {
        StartCoroutine(ChangeLevel(new List<int>()));
    }
    
    public IEnumerator ChangeLevel(List<int> avoidedSceneIndex)
    {
        menu.transform.DOScale(0, 0.5f).SetEase(Ease.OutExpo);
        next.transform.DOScale(0, 0.5f).SetEase(Ease.InExpo);
        menu.interactable = false;
        next.interactable = false;
        
        isTimeTrackingStarted = false;
        PlayerShooting.shotsFiredPerPlayer.Clear();

        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex;

        if (SceneManager.sceneCountInBuildSettings <= 2)
            nextSceneIndex = currentScene;
        else if (currentScene == 1)
            nextSceneIndex = 2;
        else
            nextSceneIndex = 1;
        
        Debug.Log("Loading next level in " + levelLoadTime + " seconds...");
        
        StartCoroutine(Countdown(levelLoadTime));
        
        DOVirtual.DelayedCall(0.3f, () =>
        {
            endGameLoading.transform.DOScale(1, 0.5f).SetEase(Ease.InExpo);
        });
        
        yield return new WaitForSecondsRealtime(levelLoadTime);
        //SceneManager.LoadScene(nextSceneIndex);
        SceneHandler.Instance.GoToScene(nextSceneIndex);
    }
    
    private IEnumerator Countdown(int countdownTime)
    {
        while (countdownTime > 0)
        {
            endGameLoading.GetComponent<TMP_Text>().text = "Loading next level in " + (countdownTime);
            yield return new WaitForSecondsRealtime(1);
            countdownTime--;
        }
    }
}
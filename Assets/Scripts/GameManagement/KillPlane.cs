using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
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
    private List<PlayerStats> deadPlayers = new();
    private List<PlayerPotrait> playerPotraits = new List<PlayerPotrait>();
    private Dictionary<int, int> timeAliveTracking = new Dictionary<int, int>();
    private static bool isTimeTrackingStarted = false;
    private bool gameIsOver = false;
    
    [SerializeField] private PlayerPotrait playerPotraitPrefab;
    
    [Header("Temp")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject endGameLoading;
    [SerializeField] private TMP_Text endGameTitle;
    
    [SerializeField] private GameObject winnerAvatar;
    [SerializeField] private TMP_Text timeAliveWinner;
    [SerializeField] private TMP_Text shotsFiredWinner;

    [SerializeField] private GameObject secondAvatar;
    [SerializeField] private TMP_Text timeAliveSecond;
    [SerializeField] private TMP_Text shotsFiredSecond;

    [SerializeField] private GameObject thirdAvatar;
    [SerializeField] private TMP_Text timeAliveThird;
    [SerializeField] private TMP_Text shotsFiredThird;

    [SerializeField] private GameObject fourthAvatar;
    [SerializeField] private TMP_Text timeAliveFourth;
    [SerializeField] private TMP_Text shotsFiredFourth;
    
    [SerializeField] private List<AudioClip> playerDeathSounds;

    [SerializeField] private int levelLoadTime = 3;

    [SerializeField] private Button menu;
    [SerializeField] private Button next;

    // TEMP
    private void Awake()
    {
        Time.timeScale = 1.0f;
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
        if (gameIsOver) return;

        if(other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            if (!players.Contains(player))
            {
                players.Add(player);
            }
            
            player.lives--;

            AudioHandler.PlayRandomEffectFromList(playerDeathSounds);

            if (player.lives <= 0)
            {
                player.lives = 0;
            }
            
            if(!player.alive)
            {
                if (!deadPlayers.Contains(player))
                {
                    deadPlayers.Add(player);
                    //player.playerIndex = deadPlayers.Count - 1;
                }
            }

            if(player.alive)
            {
                // Respawn
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
                if(rb != null) rb.velocity = Vector3.zero;
            }
            else
            {
                // Check how many players are remaining
                PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
                List<PlayerStats> alivePlayers = new();
                foreach (PlayerStats p in allPlayers)
                {
                    if (p.alive) alivePlayers.Add(p);
                }

                try
                {
                    List<Transform> alivePlayerTransforms = new();  
                    foreach (var item in alivePlayers)
                    {
                        alivePlayerTransforms.Add(item.transform);
                    }
                    CameraController.Instance.StartTrackingObjects(alivePlayerTransforms);
                }
                catch
                {
                    // No camera controller
                }

                if (alivePlayers.Count < 1)
                {
                    // TODO: Tie
                    gameIsOver = true;
                    Debug.LogError("No implementation for a TIED game");

                    EndGameTie();
                }
                else if (alivePlayers.Count == 1)
                {
                    // Winner
                    EndGame(alivePlayers[0]);
                }

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
                player.gameObject.SetActive(false);
            }
        }
        else
        {
            // Destroy(other.gameObject);
        }
    }

    private void EndGame(PlayerStats winner)
    {
        if (endGameCanvas == null) return;
        gameIsOver = true;

        if (!deadPlayers.Contains(winner))
        {
            deadPlayers.Add(winner);
        }
        
        GameObject[] avatars = new GameObject[] { winnerAvatar, secondAvatar, thirdAvatar, fourthAvatar };
        
        playerPotraits.Clear();
        
        for (int i = 0; i < deadPlayers.Count; i++)
        {
            PlayerStats playerStats = deadPlayers[i];

            PlayerPotrait playerPotraitInstance = Instantiate(playerPotraitPrefab, avatars[deadPlayers.Count - 1 - i].transform);

            playerPotraitInstance.background.color = playerStats.colors[playerStats.playerIndex];
            playerPotraitInstance.playerPotrait.sprite = playerStats.playerSprites[playerStats.playerIndex];
            playerPotraitInstance.damagePercentage.color = playerStats.colors[playerStats.playerIndex];
            
            playerPotraitInstance.damagePercentage.text = "0%";
            playerPotraitInstance.transform.localScale = Vector3.zero;
            playerPotraitInstance.gameObject.SetActive(true);
            playerPotraits.Add(playerPotraitInstance);
        }

        ShowEndGamePanel(winner.playerIndex);
    }

    private void EndGameTie()
    {
        if(endGameCanvas == null) return;
        gameIsOver = true;

        ShowEndGamePanel(0);
    }

    private void ShowEndGamePanel(int winnerIndex)
    {
        foreach (PlayerUIHandler playerUIHandler in GameStartManager.playerUIHandlers)
        {
            playerUIHandler.HidePlayerPotraits();
        }
        
        List<TMP_Text> timeAliveTexts = new List<TMP_Text> { timeAliveWinner, timeAliveSecond, timeAliveThird, timeAliveFourth };
        List<TMP_Text> shotsFiredTexts = new List<TMP_Text> { shotsFiredWinner, shotsFiredSecond, shotsFiredThird, shotsFiredFourth };

        endGameCanvas.SetActive(true);
        endGameTitle.text = "Player " + (winnerIndex + 1) + " Wins!";
        endGameTitle.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);

        DOVirtual.DelayedCall(1f, () =>
        {
            endGamePanel.transform.DOScale(1, 1f).SetEase(Ease.OutQuart);
        });

        int playerCount = Math.Min(deadPlayers.Count, timeAliveTexts.Count);
        playerCount = Math.Min(playerCount, shotsFiredTexts.Count);
        playerCount = Math.Min(playerCount, playerPotraits.Count);
        
        List<PlayerPotrait> reversedPlayerPotraits = new List<PlayerPotrait>(playerPotraits);
        reversedPlayerPotraits.Reverse();

        for (int i = 0; i < playerCount; i++)
        {
            int index = i;
            PlayerStats playerStats = deadPlayers[deadPlayers.Count - 1 - index];
            PlayerPotrait playerPotrait = reversedPlayerPotraits[i];
            
            float delay = i == 0 ? 1.5f : 3.0f + (0.1f * (index - 1));

            DOVirtual.DelayedCall(delay + 0.1f, () =>
            {
                playerPotrait.transform.DOScale(0.6f, 0.7f).SetEase(Ease.OutBounce);
            });

            DOVirtual.DelayedCall(delay + 0.3f, () =>
            {
                timeAliveTexts[index].text = $"Survived {playerStats.timeAlive} s";
                timeAliveTexts[index].transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            });

            DOVirtual.DelayedCall(delay + 0.6f, () =>
            {
                int shotsFired = PlayerShooting.shotsFiredPerPlayer.ContainsKey(playerStats.playerIndex) ? PlayerShooting.shotsFiredPerPlayer[playerStats.playerIndex] : 0;
                shotsFiredTexts[index].text = $"Fired {shotsFired} shots";
                shotsFiredTexts[index].transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            });
        }

        DOVirtual.DelayedCall(5f, () =>
        {
            menu.interactable = true;
            next.interactable = true;
            
            EventSystem eventSystem = FindObjectOfType<EventSystem>();
            eventSystem.SetSelectedGameObject(next.gameObject);
            
            menu.transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            next.transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
        });
        
    }
    
    public void OnMenuButtonPress()
    {
        SceneManager.LoadScene(0);
    }
    
    public void OnNextButtonPress()
    {
        StartCoroutine(ChangeLevel(new List<int>()));
    }
    
    public IEnumerator ChangeLevel(List<int> avoidedSceneIndex)
    {
        menu.transform.DOScale(0, 0.7f).SetEase(Ease.OutBounce);
        next.transform.DOScale(0, 0.7f).SetEase(Ease.OutBounce);
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
        
        DOVirtual.DelayedCall(0.2f, () =>
        {
            endGameLoading.transform.DOScale(1, 0.7f).SetEase(Ease.OutExpo);
        });
        
        yield return new WaitForSecondsRealtime(levelLoadTime);
        SceneManager.LoadScene(nextSceneIndex);
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
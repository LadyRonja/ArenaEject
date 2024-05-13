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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class KillPlane : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new();
    public static Dictionary<int, int> shotsFiredTracking = new Dictionary<int, int>();
    private Dictionary<int, int> timeAliveTracking = new Dictionary<int, int>();
    private bool gameIsOver = false;

    [Header("Temp")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private TMP_Text endGameTitle;
    
    [SerializeField] private GameObject winnerAvatar;
    [SerializeField] private GameObject secondAvatar;
    [SerializeField] private GameObject thirdAvatar;
    [SerializeField] private GameObject fourthAvatar;
    
    [SerializeField] private TMP_Text shotsFiredWinner;
    [SerializeField] private TMP_Text shotsFiredSecond;
    [SerializeField] private TMP_Text shotsFiredThird;
    [SerializeField] private TMP_Text shotsFiredFourth;
    
    [SerializeField] private TMP_Text timeAliveWinner;
    [SerializeField] private TMP_Text timeAliveSecond;
    [SerializeField] private TMP_Text timeAliveThird;
    [SerializeField] private TMP_Text timeAliveFourth;
    
    [SerializeField] private GameObject endGamePanel;
    [SerializeField] private GameObject endGameLoading;
    [SerializeField] private List<AudioClip> playerDeathSounds;

    [SerializeField] private int levelLoadTime = 3;

    // TEMP
    private void Awake()
    {
        Time.timeScale = 1.0f;
        shotsFiredTracking = new();
    }

    private void Start()
    {
        StartCoroutine(IncrementTimeAlive());
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
            player.lives--;

            AudioHandler.PlayRandomEffectFromList(playerDeathSounds);

            if (player.lives <= 0)
            {
                player.lives = 0;
            }
            
            if(!player.alive)
            {
                if (!timeAliveTracking.ContainsKey(player.playerIndex))
                {
                    timeAliveTracking[player.playerIndex] = player.timeAlive;
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
                        shotsFiredTracking.Add(player.playerIndex, dyingUser.shotsFired);
                    }
                    catch 
                    {
                        shotsFiredTracking[player.playerIndex] = dyingUser.shotsFired;
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
        if(endGameCanvas == null) return;
        gameIsOver = true;

        // Check if JoinScreenManager and playerConfigs are initialized
        if (StaticStuff.Instance.JoinScreenManager == null)
        {
            StaticStuff.Instance.JoinScreenManager = gameObject.AddComponent<JoinScreenManager>();
        }
        if (StaticStuff.Instance.JoinScreenManager.playerConfigs == null)
        {
            StaticStuff.Instance.JoinScreenManager.playerConfigs = new List<PlayerConfigurations>();
        }

        if (StaticStuff.Instance.JoinScreenManager.playerConfigs.Count >= 1)
        {
            for (int i = 0; i < StaticStuff.Instance.JoinScreenManager.playerConfigs.Count; i++)
            {
                PlayerConfigurations playerConfig = StaticStuff.Instance.JoinScreenManager.playerConfigs[i];

                switch (i)
                {
                    case 0:
                        winnerAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 1:
                        secondAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 2:
                        thirdAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 3:
                        fourthAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                }
            }
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
        List<PlayerStats> sortedPlayersByTimeAlive =
            ((PlayerStats[])FindObjectsOfType<PlayerStats>()).OrderByDescending(player => player.timeAlive).ToList();

        foreach (PlayerStats player in sortedPlayersByTimeAlive)
        {
            if(player.TryGetComponent<WeaponUser>(out WeaponUser aliveUser)){
                shotsFiredTracking.Add(player.playerIndex, aliveUser.shotsFired);
            }
        }
        
        // Aktivera panelen
        endGameCanvas.SetActive(true);
        DOVirtual.DelayedCall(0.5f, () =>
        {
            {
                // Bestäm vinnare
                if (winnerIndex == 0)
                {
                    // gameOverWinnerText.text = "Tie!";
                    endGameTitle.text = "Player " + (winnerIndex + 1) + " Wins!";
                }
                else
                {
                    endGameTitle.text = "Player " + (winnerIndex + 1) + " Wins!";
                }

                // Animation för vinnarens namn
                endGameTitle.transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic);

                // Panel animation
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    endGamePanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutQuart).onComplete += () =>
                        endGameTitle.transform.DOScale(1, 0.5f).SetEase(Ease.OutElastic);
                    
                    // Avatar animation
                    DOVirtual.DelayedCall(0.5f,
                        () => { winnerAvatar.transform.DOScale(2, 0.5f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.7f,
                        () => { secondAvatar.transform.DOScale(2, 0.5f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.9f,
                        () => { thirdAvatar.transform.DOScale(2, 0.5f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(1.1f,
                        () => { fourthAvatar.transform.DOScale(2, 0.5f).SetEase(Ease.OutElastic); });
                    
                    // Stats
                    for (int i = 0; i < sortedPlayersByTimeAlive.Count; i++)
                    {
                        PlayerStats player = sortedPlayersByTimeAlive[i];
                        int shotsFired = shotsFiredTracking.ContainsKey(player.playerIndex) ? shotsFiredTracking[player.playerIndex] : 0;

                        switch (i)
                        {
                            case 0:
                                timeAliveWinner.text = "";
                                shotsFiredWinner.text = $"Fired {shotsFired} shots";
                                break;
                            case 1:
                                timeAliveSecond.text = $"Survived {player.timeAlive} s";
                                shotsFiredSecond.text = $"Fired {shotsFired} shots";
                                break;
                            case 2:
                                timeAliveThird.text = $"Survived {player.timeAlive} s";
                                shotsFiredThird.text = $"Fired {shotsFired} shots";
                                break;
                            case 3:
                                timeAliveFourth.text = $"Survived {player.timeAlive} s";
                                shotsFiredFourth.text = $"Fired {shotsFired} shots";
                                break;
                        }
                    }
                    
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        timeAliveWinner.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        timeAliveSecond.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.4f, () =>
                    {
                        timeAliveThird.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.6f, () =>
                    {
                        timeAliveFourth.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        shotsFiredWinner.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        shotsFiredSecond.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.4f, () =>
                    {
                        shotsFiredThird.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.6f, () =>
                    {
                        shotsFiredFourth.transform.DOScale(0.5f, 0.5f).SetEase(Ease.OutElastic);
                    });
                
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        StartCoroutine(ChangeLevel(new List<int> {0, 1}));
                    });
                });
            }
        });
    }
    
    private IEnumerator ChangeLevel(List<int> avoidedSceneIndex)
    {
        PlayerShooting.shotsFiredPerPlayer.Clear();

        int sceneCount = SceneManager.sceneCountInBuildSettings;
        int currentLevelIndex = SceneManager.GetActiveScene().buildIndex;

        List<int> allSceneIndices = Enumerable.Range(0, sceneCount).ToList();

        allSceneIndices.Remove(currentLevelIndex);
        allSceneIndices.RemoveAll(i => avoidedSceneIndex.Contains(i));

        if (allSceneIndices.Count == 0)
        {
            Debug.Log("This is the only level available in the builds settings. Reloading in " + levelLoadTime + " seconds...");

            StartCoroutine(Countdown(levelLoadTime + 2));
            
            DOVirtual.DelayedCall(2f, () =>
            {
                endGameLoading.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
            });
            
            yield return new WaitForSecondsRealtime(levelLoadTime + 2);
            SceneManager.LoadScene(currentLevelIndex);
        }
        else
        {
            Debug.Log("Loading next level in " + levelLoadTime + " seconds...");
            
            int nextSceneIndex = allSceneIndices[Random.Range(0, allSceneIndices.Count)];
            StartCoroutine(Countdown(levelLoadTime + 2));
            
            DOVirtual.DelayedCall(2f, () =>
            {
                endGameLoading.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
            });
            
            yield return new WaitForSecondsRealtime(levelLoadTime + 2);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private IEnumerator Countdown(int countdownTime)
    {
        while (countdownTime > 0)
        {
            endGameLoading.GetComponent<TMP_Text>().text = "Loading next level in " + countdownTime;
            yield return new WaitForSecondsRealtime(1);
            countdownTime--;
        }
    }
}
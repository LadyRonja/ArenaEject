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
    private bool gameIsOver = false;

    [Header("Temp")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private TMP_Text endGameTitle;
    [SerializeField] private GameObject endGameWinnerAvatar;
    [SerializeField] private GameObject endGameSecondAvatar;
    [SerializeField] private GameObject endGameThirdAvatar;
    [SerializeField] private GameObject endGameFourthAvatar;
    [SerializeField] private TMP_Text endGameWinnerShotsFired;
    [SerializeField] private TMP_Text endGameSecondShotsFired;
    [SerializeField] private TMP_Text endGameThirdShotsFired;
    [SerializeField] private TMP_Text endGameFourthShotsFired;
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
            Destroy(other.gameObject);
        }
    }

    private void EndGame(PlayerStats winner)
    {
        if(endGameCanvas == null) return;
        gameIsOver = true;
        //Time.timeScale = 0.2f;

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
                        endGameWinnerAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 1:
                        endGameSecondAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 2:
                        endGameThirdAvatar.GetComponent<Image>().color = playerConfig.playerColor;
                        break;
                    case 3:
                        endGameFourthAvatar.GetComponent<Image>().color = playerConfig.playerColor;
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
        Time.timeScale = 0.2f;

        ShowEndGamePanel(0);
    }

    private void ShowEndGamePanel(int winnerIndex)
    {
        List<PlayerStats> remainingPlayersPlayers = 
            ((PlayerStats[])FindObjectsOfType<PlayerStats>()).ToList();

        foreach (PlayerStats player in remainingPlayersPlayers)
        {
            if(player.TryGetComponent<WeaponUser>(out WeaponUser aliveUser)){
                shotsFiredTracking.Add(player.playerIndex, aliveUser.shotsFired);
            }
        }
        
        // Aktivera panelen
        endGameCanvas.SetActive(true);
        DOVirtual.DelayedCall(0.4f, () =>
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
                endGameTitle.transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic);

                // Panel animation
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    endGamePanel.transform.DOScale(1, 0.2f).SetEase(Ease.OutQuart).onComplete += () =>
                        endGameTitle.transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic);
                    
                    // Avatar animation
                    DOVirtual.DelayedCall(0.5f,
                        () => { endGameWinnerAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.6f,
                        () => { endGameSecondAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.7f,
                        () => { endGameThirdAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.8f,
                        () => { endGameFourthAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    
                    // Avfyrade skott animation
                    string shotsFiredWinner = "";
                    string shotsFiredSecond = "";
                    string shotsFiredThird= "";
                    string shotsFiredFourth = "";
                    
                    foreach (var shotsFired in shotsFiredTracking)
                    {
                        if (shotsFired.Key == 0)
                        {
                            shotsFiredWinner = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 1)
                        {
                            shotsFiredSecond = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 2)
                        {
                            shotsFiredThird = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 3)
                        {
                            shotsFiredFourth = "Shots Fired: " + shotsFired.Value;
                        }
                    }
                    
                    endGameWinnerShotsFired.text = shotsFiredWinner;
                    endGameSecondShotsFired.text = shotsFiredThird;
                    endGameThirdShotsFired.text = shotsFiredThird;
                    endGameFourthShotsFired.text = shotsFiredFourth;
                
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        endGameWinnerShotsFired.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.1f, () =>
                    {
                        endGameSecondShotsFired.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        endGameThirdShotsFired.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.3f, () =>
                    {
                        endGameFourthShotsFired.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        StartCoroutine(ChangeLevel(new List<int> {0, 1}));
                    });
                });
            };
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
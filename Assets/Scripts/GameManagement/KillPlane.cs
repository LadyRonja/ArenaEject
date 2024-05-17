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
    private List<PlayerStats> players = new();
    private List<PlayerStats> deadPlayers = new();
    private Dictionary<int, int> timeAliveTracking = new Dictionary<int, int>();
    private static bool isTimeTrackingStarted = false;
    private bool gameIsOver = false;

    [Header("Temp")]
    [SerializeField] private GameObject endGameCanvas;
    [SerializeField] private GameObject endGamePanel;
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
    
    [SerializeField] private GameObject endGameLoading;
    [SerializeField] private List<AudioClip> playerDeathSounds;

    [SerializeField] private int levelLoadTime = 3;

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
                    player.playerIndex = deadPlayers.Count - 1;
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
        if(endGameCanvas == null) return;
        gameIsOver = true;
        
        if (!deadPlayers.Contains(winner))
        {
            deadPlayers.Add(winner);
        }

        JoinScreenManager joinScreenManager = FindObjectOfType<JoinScreenManager>();

        if (joinScreenManager.playerConfigs.Count >= 1)
        {
            for (int i = 0; i < joinScreenManager.playerConfigs.Count; i++)
            {
                PlayerConfigurations playerConfig = joinScreenManager.playerConfigs[i];

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
        List<GameObject> avatars = new List<GameObject> { winnerAvatar, secondAvatar, thirdAvatar, fourthAvatar };
        List<TMP_Text> timeAliveTexts = new List<TMP_Text> { timeAliveWinner, timeAliveSecond, timeAliveThird, timeAliveFourth };
        List<TMP_Text> shotsFiredTexts = new List<TMP_Text> { shotsFiredWinner, shotsFiredSecond, shotsFiredThird, shotsFiredFourth };

        foreach (GameObject avatar in avatars)
        {
            avatar.SetActive(false);
        }

        endGameCanvas.SetActive(true);
        endGameTitle.text = "Player " + (winnerIndex + 1) + " Wins!";
        endGameTitle.transform.DOScale(1, 1f).SetEase(Ease.OutElastic);

        DOVirtual.DelayedCall(1f, () =>
        {
            endGamePanel.transform.DOScale(1, 1f).SetEase(Ease.OutQuart);
        });

        for (int i = 0; i < deadPlayers.Count; i++)
        {
            int index = i;

            float delay = i == 0 ? 1.5f : 3.0f + (0.1f * (index - 1));

            DOVirtual.DelayedCall(delay, () =>
            {
                avatars[index].SetActive(true);
                avatars[index].transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            });

            DOVirtual.DelayedCall(delay + 0.3f, () =>
            {
                timeAliveTexts[index].text = $"Survived {deadPlayers[deadPlayers.Count - 1 - index].timeAlive} s";
                timeAliveTexts[index].transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            });

            DOVirtual.DelayedCall(delay + 0.6f, () =>
            {
                int shotsFired = PlayerShooting.shotsFiredPerPlayer.ContainsKey(deadPlayers[deadPlayers.Count - 1 - index].playerIndex) ? PlayerShooting.shotsFiredPerPlayer[deadPlayers[deadPlayers.Count - 1 - index].playerIndex] : 0;
                shotsFiredTexts[index].text = $"Fired {shotsFired} shots";
                shotsFiredTexts[index].transform.DOScale(1, 0.7f).SetEase(Ease.OutBounce);
            });
        }
        
        DOVirtual.DelayedCall(5f, () =>
        {
            StartCoroutine(ChangeLevel(new List<int> {0}));
        });
    }
        
    private IEnumerator ChangeLevel(List<int> avoidedSceneIndex)
    {
        isTimeTrackingStarted = false;
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
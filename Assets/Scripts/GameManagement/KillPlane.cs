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
using Random = UnityEngine.Random;

[RequireComponent(typeof(Collider))]
public class KillPlane : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints = new();
    private Dictionary<int, int> shotsFiredTracking = new Dictionary<int, int>();
    private bool gameIsOver = false;

    [Header("Temp")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private TMP_Text gameOverWinnerText;
    [SerializeField] private GameObject gameOverWinnerAvatar;
    [SerializeField] private GameObject gameOverPlayerTwoAvatar;
    [SerializeField] private GameObject gameOverPlayerThreeAvatar;
    [SerializeField] private GameObject gameOverPlayerFourAvatar;
    [SerializeField] private TMP_Text gameOverWinnerShotsFiredText;
    [SerializeField] private TMP_Text gameOverPlayerTwoShotsFiredText;
    [SerializeField] private TMP_Text gameOverPlayerThreeShotsFiredText;
    [SerializeField] private TMP_Text gameOverPlayerFourShotsFiredText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject gameOverLoadingText;
    [SerializeField] private List<AudioClip> playerDeathSounds;

    [SerializeField] private int levelLoadTime = 3;

    // TEMP
    private void Awake()
    {
        Time.timeScale = 1.0f;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameIsOver) return;

        if(other.TryGetComponent<PlayerStats>(out PlayerStats player))
        {
            player.lives--;

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
                    shotsFiredTracking.Add(player.playerIndex, dyingUser.shotsFired);
                }
                player.gameObject.SetActive(false);
                AudioHandler.PlayRandomEffectFromList(playerDeathSounds);
            }
        }
    }

    private void EndGame(PlayerStats winner)
    {
        if(gameOverScreen == null) return;
        gameIsOver = true;
        Time.timeScale = 0.2f;
        
        ShowEndGamePanel(winner.playerIndex);
    }
    
    private void EndGameTie()
    {
        if(gameOverScreen == null) return;
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
        gameOverScreen.SetActive(true);
        DOVirtual.DelayedCall(0.4f, () =>
        {
            {
                // Bestäm vinnare
                if (winnerIndex == 0)
                {
                    // gameOverWinnerText.text = "Tie!";
                    gameOverWinnerText.text = "Player " + (winnerIndex + 1) + " Wins!";
                }
                else
                {
                    gameOverWinnerText.text = "Player " + (winnerIndex + 1) + " Wins!";
                }

                // Namn animation
                gameOverWinnerText.transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic);

                // Panel animation
                DOVirtual.DelayedCall(0.5f, () =>
                {
                    gameOverPanel.transform.DOScale(1, 0.2f).SetEase(Ease.OutQuart).onComplete += () =>
                        gameOverWinnerText.transform.DOScale(1, 0.2f).SetEase(Ease.OutElastic);
                    
                    // Avatar animation
                    DOVirtual.DelayedCall(0.5f,
                        () => { gameOverWinnerAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.6f,
                        () => { gameOverPlayerTwoAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.7f,
                        () => { gameOverPlayerThreeAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    DOVirtual.DelayedCall(0.8f,
                        () => { gameOverPlayerFourAvatar.transform.DOScale(2, 0.2f).SetEase(Ease.OutElastic); });
                    
                    // Avfyrade skott animation
                    string shotsFiredWinnerText = "";
                    string shotsFiredPlayerTwoText = "";
                    string shotsFiredPlayerThreeText = "";
                    string shotsFiredPlayerFourText = "";
                    
                    // Use shotsFiredTracking to get the shots fired for each player
                    foreach (var shotsFired in shotsFiredTracking)
                    {
                        if (shotsFired.Key == 0)
                        {
                            shotsFiredWinnerText = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 1)
                        {
                            shotsFiredPlayerTwoText = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 2)
                        {
                            shotsFiredPlayerThreeText = "Shots Fired: " + shotsFired.Value;
                        }
                        else if (shotsFired.Key == 3)
                        {
                            shotsFiredPlayerFourText = "Shots Fired: " + shotsFired.Value;
                        }
                    }
                    
                    gameOverWinnerShotsFiredText.text = shotsFiredWinnerText;
                    gameOverPlayerTwoShotsFiredText.text = shotsFiredPlayerTwoText;
                    gameOverPlayerThreeShotsFiredText.text = shotsFiredPlayerThreeText;
                    gameOverPlayerFourShotsFiredText.text = shotsFiredPlayerFourText;
                
                    DOVirtual.DelayedCall(1f, () =>
                    {
                        gameOverWinnerShotsFiredText.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.1f, () =>
                    {
                        gameOverPlayerTwoShotsFiredText.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.2f, () =>
                    {
                        gameOverPlayerThreeShotsFiredText.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
                    });
                    DOVirtual.DelayedCall(1.3f, () =>
                    {
                        gameOverPlayerFourShotsFiredText.transform.DOScale(0.5f, 0.2f).SetEase(Ease.OutElastic);
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
                gameOverLoadingText.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
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
                gameOverLoadingText.transform.DOScale(1, 0.2f).SetEase(Ease.OutExpo);
            });
            
            yield return new WaitForSecondsRealtime(levelLoadTime + 2);
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private IEnumerator Countdown(int countdownTime)
    {
        while (countdownTime > 0)
        {
            gameOverLoadingText.GetComponent<TMP_Text>().text = "Loading next level in " + countdownTime;
            yield return new WaitForSecondsRealtime(1);
            countdownTime--;
        }
    }
}
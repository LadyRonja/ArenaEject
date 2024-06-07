using System;
using System.Collections.Generic;
using Coffee.UIExtensions;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Linq;

public class EndGameManager : MonoBehaviour
{
    private static EndGameManager instance;
    public static EndGameManager Instance { get { return GetInstance(); } }

    List<PlayerStats> deadPlayers = new();
    [NonSerialized] public bool gameIsOver = false;
    [SerializeField] private GameObject endGameCanvasPrefab;
    [SerializeField] private TempPLayerEndPotrait tempPlayerEndPotraitPrefab;
    public List<AudioClip> deathSounds = new();

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

    private void Start()
    {

        deadPlayers.Clear();
        deadPlayers = new();
    }

    public void PlayerDied(PlayerStats player)
    {
        if (!deadPlayers.Contains(player))
        {
            player.lives = 0;
            deadPlayers.Add(player);

            AudioHandler.PlayRandomEffectFromList(deathSounds);
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

        List<Transform> transforms = new();
        foreach (PlayerStats p in alivePLayers)
        {
            transforms.Add(p.transform);
        }
        CameraController.Instance.StartTrackingObjects(transforms);

        if (alivePLayers.Count == 1)
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

        Bot[] bots = (Bot[])FindObjectsOfType(typeof(Bot), true);
        foreach (Bot b in bots)
        {
            b.enabled = false;
        }

        winner.gameObject.GetComponent<Movement>().enabled = false;
        
        if (StaticStats.playerWins.ContainsKey(winner.playerIndex))
        {
            StaticStats.playerWins[winner.playerIndex]++;
        }
        else
        {
            StaticStats.playerWins.Add(winner.playerIndex, 1);
        }

        if(endGameCanvasPrefab == null )
        {
            FailSafe("endGameCanvasPrefab missing! Instantly going to next level");
            return;
        }

        if(tempPlayerEndPotraitPrefab == null)
        {
            FailSafe("endGamePotraitProfilePrefab missing! Instantly going to next level");
            return;
        }

        GameObject gameOverScreen = Instantiate(endGameCanvasPrefab);
        TempEndGameCanvas serializedCanvasVars = gameOverScreen.GetComponent<TempEndGameCanvas>();
        Transform potraitParent = this.transform;
        try
        {
            potraitParent = serializedCanvasVars.playerGridRegion;
        }
        catch
        {
            FailSafe("Unable to find potraitParent");
            return;
        }

        List<PlayerStats> allPlayers = FindAllPlayers(true);

        serializedCanvasVars.gameOverText.transform.DOScale(1, 1f).SetEase(Ease.OutQuart);
        
        if (SceneManager.GetActiveScene().name == Paths.FARRAZ_SCENE_NAME)
        {
            serializedCanvasVars.BG.GetComponent<Image>().color = new Color(1f, 0f, 0.2484708f, 0.9f);
        }
        else
        {
            Color newColor = Color.magenta;
            newColor.a = 0.9f;
            serializedCanvasVars.BG.GetComponent<Image>().color = newColor;//Color.magenta.WithAlpha(0.9f);
        }
        
        DOVirtual.DelayedCall(1f, () =>
        {
            serializedCanvasVars.BG.transform.DOScale(1, 1f).SetEase(Ease.OutQuart);
            
        });

        int index = 0;
        
        List<PlayerStats> temp = new List<PlayerStats>();
        temp.Add(winner);
        temp.AddRange(deadPlayers);
        temp = temp.Select(p => p).Distinct().ToList();

        Debug.Log("player count: " + temp.Count);
        foreach (PlayerStats ps in temp)
        {
            Debug.Log(ps.playerIndex + ps.name);
        }
        
        foreach (PlayerStats p in temp)
        {
            TempPLayerEndPotrait potrait = Instantiate(tempPlayerEndPotraitPrefab, potraitParent);
            
            potrait.background.color = p.colors[p.playerIndex];
            potrait.playerPotrait.sprite = p.endGameSprite;
            potrait.playerWins.text = StaticStats.playerWins.ContainsKey(p.playerIndex) ? $"{StaticStats.playerWins[p.playerIndex]} Wins" : "0 Wins";

            float delay = index == 0 ? 1.5f : 3.5f + (0.1f * (index - 1));

            DOVirtual.DelayedCall(delay + 0.1f, () =>
            {
                var tween = potrait.transform.DOScale(0.6f, 0.5f).SetEase(Ease.InOutQuint);
                if (p == winner)
                {
                    tween.OnComplete(() =>
                    {
                        Transform bgTransform = potrait.transform.Find("BG");
                        if (bgTransform != null)
                        {
                            UIShiny shineEffect = bgTransform.GetComponent<UIShiny>();
                            if (shineEffect != null)
                            {
                                shineEffect.Play();
                            }
                            else
                            {
                                Debug.LogError("UIShiny component not found on BG GameObject");
                            }
                        }
                        else
                        {
                            Debug.LogError("BG GameObject not found");
                        }
            
                        potrait.playerWins.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
                    });
                }

                else
                {
                    tween.OnComplete(() =>
                    {
                        
                        potrait.playerWins.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
                    });
                }
            });

            index++;
        }

        DOVirtual.DelayedCall(5f, () =>
        {
            serializedCanvasVars.menuButton.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
            serializedCanvasVars.againButton.transform.DOScale(1, 0.5f).SetEase(Ease.InOutQuint);
        });

        //Invoke(nameof(GoToNextScene), 5f);

        void FailSafe(string debugMsg)
        {
            Debug.Log(debugMsg);
            int randomLevel = UnityEngine.Random.Range(1, 3);
            SceneHandler.Instance.GoToScene(randomLevel);
        }
    }

    public void GoToNextScene()
    {
        int randomLevel = UnityEngine.Random.Range(1, 3);
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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinScreenManager : MonoBehaviour
{
    [HideInInspector] public List<PlayerConfigurations> playerConfigs = new();
    //[SerializeField] private int maxPLayers = 4;

    //[SerializeField] private GameObject playerDisplayPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform displayParent;
    [SerializeField] private List<Color> playerColors = new List<Color>();
    private List<GameObject> spawnPoints = new();

    private static JoinScreenManager instance;
    public static JoinScreenManager Instance { get => instance; }

    int timesLoaded = 0; // awful solution to subscribing multiple times

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            playerConfigs = new();
            if(timesLoaded == 0)
            {
                SceneManager.sceneLoaded += delegate { OnSceneLoad(); };
            }
            timesLoaded++;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void OnSceneLoad()
    {
        // When entering a new scene, wcheck if the new scene is the start screen
        if (SceneManager.GetActiveScene().name == Paths.START_SCENE_NAME)
        {
            // Cleanup things that should be resey
            GameStartManager.KillAllPlayers();
            DestroyChildren();
            playerConfigs = new();
            spawnPoints = new();
            if(displayParent != null)
            {
                List<Transform> children = new();
                foreach (Transform t in displayParent.transform) { 
                    children.Add(t);
                }

                foreach (Transform child in children)
                {
                    Destroy(child.gameObject);
                }
            }

            // Find things that should be in the scene
            displayParent = FindObjectOfType<JoinDisplayParent>(true).transform;
            spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint").ToList();

        }
        else
        {
            PlayerInputManager.instance.DisableJoining();      
        }

        void DestroyChildren()
        {
            int i = 0;

            //Array to hold all child obj
            GameObject[] allChildren = new GameObject[transform.childCount];

            //Find all child obj and store to that array
            foreach (Transform child in transform)
            {
                allChildren[i] = child.gameObject;
                i += 1;
            }

            //Now destroy them
            foreach (GameObject child in allChildren)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }


    private void OnDestroy()
    {
        // Make sure to unsubscribe from events on destroy (game end)
        // Otherwise these subscriptions will persist between sessions during development
        SceneManager.sceneLoaded -= delegate { OnSceneLoad(); };
    }


    public void HandlePlayerJoined(PlayerInput pi)
    {
        if(SceneManager.GetActiveScene().name != Paths.START_SCENE_NAME)
        {
            return;
        }

        Debug.Log("Player joined: " + pi.playerIndex);
        pi.transform.SetParent(this.transform);
        if(!playerConfigs.Any(p => p.playerIndex == pi.playerIndex))
        {
            PlayerConfigurations playerConfig = new PlayerConfigurations(pi);
            playerConfigs.Add(playerConfig);
           if(displayParent == null || displayParent == this.transform)
            {
                Debug.LogError("No display parent found! Parenting to self");
                displayParent = this.transform;
            }

            //GameObject displayObj = Instantiate(playerDisplayPrefab, Vector3.zero, Quaternion.identity, displayParent);
            //JoinDisplay displayComponents = displayObj.GetComponent<JoinDisplay>();
            //displayComponents.playerIndex = pi.playerIndex;
           //displayComponents.playerText.text = $"Player {pi.playerIndex + 1}";
            if (playerColors[pi.playerIndex] != null)
            {
                //displayComponents.displayImage.color = playerColors[pi.playerIndex];
                playerConfigs[pi.playerIndex].playerColor = playerColors[pi.playerIndex];
            }

            PlayerInput player = GameStartManager.SpawnAPlayer(playerPrefab, pi.playerIndex, playerConfig.input.devices[0]);
            GameStartManager.VerifyPlayer(player.gameObject, pi.playerIndex, true);
            player.transform.position = spawnPoints[playerConfigs.Count - 1].transform.position;

            UpdateCameraTracking();
        }
    }

    private void UpdateCameraTracking()
    {
        if (CameraController.Instance == null) return;

        var players = FindObjectsOfType<PlayerStats>().ToList();
        List<Transform> playerTransforms = new List<Transform>();
        foreach (var t in players)
        {
            playerTransforms.Add(t.transform);
        }

        if (playerTransforms.Count < 1) { return; }
        CameraController.Instance.StartTrackingObjects(playerTransforms);
    }

    public void RemovePlayerFromLobby(int playerIndex)
    {
        PlayerConfigurations playerToRemove;
        try
        {
            // Remove player config
            playerToRemove = playerConfigs.Where(p => p.playerIndex == playerIndex).ToList()[0];
            playerConfigs.Remove(playerToRemove);

            // Remove player input
            foreach (Transform child in transform)
            {
                if(child.TryGetComponent<PlayerInput>(out PlayerInput pi))
                {
                    if(pi.splitScreenIndex == playerIndex)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }

            // Remove player icon
            foreach (Transform child in displayParent)
            {
                if (child.TryGetComponent<JoinDisplay>(out JoinDisplay jp))
                {
                    if (jp.playerIndex == playerIndex)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }

            // Remove player object
            PlayerStats[] players = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
            if(players.Length > 0) {
                foreach (PlayerStats player in players)
                {
                    if(player.playerIndex == playerIndex)
                    {
                        Destroy(player.gameObject);
                        break;
                    }
                }
            }
            else
            {
                Debug.LogError("Last player in lobby does not have index 0, this should be impossible");
                StartScreenManager.Instance.GoToDefault();
            }

        }
        catch 
        {
            Debug.LogError("Whopsie doodle");
        }
    }

    public void UpdatePlayerConfigModel(int playerIndex, int newModelIndex)
    {
        try
        {
            PlayerConfigurations playerToUpdate = playerConfigs.Where(i => i.playerIndex == playerIndex).First();
            playerToUpdate.playerModelIndex = newModelIndex;
            Debug.Log("playerconfig: " + playerToUpdate.playerIndex + " now has model index: " + playerToUpdate.playerModelIndex);

        }
        catch 
        {
            Debug.LogError("Unable to update player config model data");
        }
    }
}

public class PlayerConfigurations
{
    public PlayerConfigurations(PlayerInput pi)
    {
        input= pi;
        playerIndex = pi.playerIndex;
    }

    public PlayerInput input;
    public int playerIndex;
    public Color playerColor = Color.white;
    public int playerModelIndex = 0;
}
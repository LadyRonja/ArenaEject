using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class JoinScreenManager : MonoBehaviour
{
    [HideInInspector] public List<PlayerConfigurations> playerConfigs = new();
    [SerializeField] private int maxPLayers = 4;

    [SerializeField] private GameObject playerDisplayPrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform displayParent;
    [SerializeField] private List<Color> playerColors = new List<Color>();
    private List<GameObject> spawnPoints = new();

    private static JoinScreenManager instance;
    public static JoinScreenManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            StaticStuff.Instance.JoinScreenManager = this;
            playerConfigs = new();
            SceneManager.sceneLoaded += delegate { OnSceneLoad(); };
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


    public void ToggleReady(int index)
    {
        playerConfigs[index].isReady = !playerConfigs[index].isReady;
        if (playerConfigs.Count > 1 && playerConfigs.All(p => p.isReady))
        {
            var basicButtons = (BasicButton[])FindObjectsOfType<BasicButton>();
            foreach (var buttonScripts in basicButtons)
            {
                if(buttonScripts.MyType == BasicButton.ButtonType.START)
                {
                    if(buttonScripts.TryGetComponent<Button>(out Button btn))
                    {
                        btn.interactable = true;
                    }
                }
            }
        }
    }

    public void HandlePlayerJoined(PlayerInput pi)
    {
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

            GameObject displayObj = Instantiate(playerDisplayPrefab, Vector3.zero, Quaternion.identity, displayParent);
            JoinDisplay displayComponents = displayObj.GetComponent<JoinDisplay>();
            displayComponents.playerText.text = $"Player {pi.playerIndex + 1}";
            if (playerColors[pi.playerIndex] != null)
            {
                displayComponents.displayImage.color = playerColors[pi.playerIndex];
                playerConfigs[pi.playerIndex].playerColor = playerColors[pi.playerIndex];
            }

            PlayerInput player = GameStartManager.SpawnAPlayer(playerPrefab, pi.playerIndex, playerConfig.input.devices[0]);
            GameStartManager.VerifyPlayer(player.gameObject, pi.playerIndex, true);
            player.transform.position = spawnPoints[playerConfigs.Count - 1].transform.position;

            UpdateCameraTracking();

            Debug.Log($"Player {pi.playerIndex} instantly toggled ready - disable when character select is enabled");
            ToggleReady(pi.playerIndex);
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
        Debug.LogError("RemovePlayerFromLobby not implemented, returning");
        return;

        PlayerConfigurations playerToRemove;
        try
        {
            playerToRemove = playerConfigs.Where(p => p.playerIndex == playerIndex).ToList()[0];
            playerConfigs.Remove(playerToRemove);
            PlayerStats[] players = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
            if(players.Length > 0) {
                foreach (PlayerStats player in players)
                {
                    if(player.playerIndex == playerIndex)
                    {
                        Destroy(player);
                        break;
                    }
                }
            }

        }
        catch 
        {

        }
    }

}

public class PlayerConfigurations
{
    public PlayerConfigurations(PlayerInput pi)
    {
        input= pi;
        playerIndex = pi.playerIndex;
        isReady = false;
    }

    public PlayerInput input;
    public int playerIndex;
    public Color playerColor = Color.white;
    public bool isReady;
}
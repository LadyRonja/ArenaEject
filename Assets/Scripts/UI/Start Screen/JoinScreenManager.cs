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
    [SerializeField] private Transform displayParent;
    [SerializeField] private List<Color> playerColors = new List<Color>();

    private static JoinScreenManager instance;
    public static JoinScreenManager Instance { get => instance; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            playerConfigs = new();
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnEnable()
    {
        if(SceneManager.GetActiveScene().name == Paths.START_SCENE_NAME)
        {
            playerConfigs = new();
        }
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
            playerConfigs.Add(new PlayerConfigurations(pi));
           
            GameObject displayObj = Instantiate(playerDisplayPrefab, Vector3.zero, Quaternion.identity, displayParent);
            JoinDisplay displayComponents = displayObj.GetComponent<JoinDisplay>();
            displayComponents.playerText.text = $"Player {pi.playerIndex + 1}";
            if (playerColors[pi.playerIndex] != null)
            {
                displayComponents.displayImage.color = playerColors[pi.playerIndex];
                playerConfigs[pi.playerIndex].playerColor = playerColors[pi.playerIndex];
            }

            Debug.Log($"Player {pi.playerIndex} instantly toggled ready - disable when character select is enabled");
            ToggleReady(pi.playerIndex);
        }
    }

    #region Old
    /*
    private static JoinScreenManager instance;
    public static JoinScreenManager Instance { get => instance; }

    [SerializeField] [Range(2, 8)] private int maxPlayersAllowed = 4;
    [Space]
    [SerializeField] private GameObject playerDisplayPrefab;
    [SerializeField] private Transform displayParent;
    [SerializeField] private Button startButton;
    [Space]
    [SerializeField] private List<Color> playerColors = new List<Color>();

    private List<string> playerControllersJoined = new List<string>();
    private Dictionary<int, int> playerToControllerIndex= new Dictionary<int, int>();
    public Dictionary<int, int> PlayerToControllerIndex { get => playerToControllerIndex;}

    private void OnEnable()
    {
        if(instance == null || instance == this)
        {
            instance = this;
            playerControllersJoined = new List<string>();
            playerToControllerIndex = new Dictionary<int, int>();
            maxPlayersAllowed = Math.Clamp(maxPlayersAllowed, 2, 8);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    /*
    private void Update()
    {
        CheckForJoiningPlayers();
        GeneratePrefabsForPlayers();
        DetermineIfStartConditionMet();
    }
   
    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Debug.Log("Player joined!");
        if(!(playerControllersJoined.Count < maxPlayersAllowed)) { 
            Destroy(playerInput.gameObject); 
            return; 
        }

        playerInput.transform.SetParent(displayParent);
        int playerIndex = playerInput.playerIndex;
        int gamepadIndex = -1;
        foreach (Gamepad gamepad in Gamepad.all)
        {
            if(gamepad == Gamepad.all[playerInput.playerIndex])
            {
                gamepadIndex = playerInput.playerIndex;
                break;
            }
        }
        Debug.Log("PlayerIndex: " + playerIndex);
        Debug.Log("Gamepladindex: " + gamepadIndex);
    }




    /// <summary>
    /// TODO: Allow higher number of controls to join, despite max player count
    /// </summary>
    private void CheckForJoiningPlayers()
    {
        for (int i = 1; i < maxPlayersAllowed + 1; i++)
        {
            if (Input.GetButtonDown($"P{i}_Join_Duo"))
            {
                string inputDevice = $"InputDevice_{i}";
                if (playerControllersJoined.Contains(inputDevice)) return;

                playerToControllerIndex.Add(playerControllersJoined.Count + 1, i);
                playerControllersJoined.Add(inputDevice);
            }
        }
    }

    private void GeneratePrefabsForPlayers()
    {
        if (displayParent == null) return;
        if (displayParent.childCount == playerControllersJoined.Count) return;

        // Clear display
        // TODO: If players get to select avatars/colors/hats, then this needs to be saved/reworked
        List<GameObject> displayParentChildren = new List<GameObject>();
        for (int i = 0; i < displayParent.childCount; i++)
        {
            displayParentChildren.Add(displayParent.GetChild(i).gameObject);
        }
        foreach (GameObject child in displayParentChildren)
        {
            Destroy(child);
        }

        // Generate new display
        for (int i = 1; i < playerControllersJoined.Count+1; i++)
        {
            GameObject displayObj = Instantiate(playerDisplayPrefab, Vector3.zero, Quaternion.identity, displayParent);
            JoinDisplay displayComponents = displayObj.GetComponent<JoinDisplay>();
           
            if (!playerToControllerIndex.TryGetValue(i, out int controllerIndex))
            {
                Debug.LogError(playerControllersJoined[i] + "not included in dictonary, state mutated unexpectantly");
                return;
            }
            
            displayComponents.playerText.text = $"Player {i}";
            displayComponents.controllerText.text = $"Controller {controllerIndex}";
            if (playerColors[i-1] != null) {
                displayComponents.displayImage.color = playerColors[i-1];
            }
        }
    }

    /// <summary>
    /// Determine if two or more players have joined
    /// </summary>
    private void DetermineIfStartConditionMet()
    {
        if (startButton == null) return;

        if(playerControllersJoined.Count >= 2)
        {
            startButton.interactable = true;
        }
        else
        {
            startButton.interactable = false;
        }
    }
    */
    #endregion
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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public delegate void AllPlayersSpawned();
public class GameStartManager : MonoBehaviour
{
    private static GameStartManager instance;
    public static GameStartManager Instance { get => instance; }


    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Development options")]
    [SerializeField] private bool createAPlayerForEachGamepad = false;
    [SerializeField] [Range(0,4)] private int playerCountToSpawn = 2;


    public AllPlayersSpawned OnAllPlayersSpawned;


    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        SpawnAllPlayers();
        Invoke(nameof(StartCameraTracking), 0.2f);
    }

    private void SpawnAllPlayers()
    {
        if (playerPrefab == null) return;

        // Clear existing players
        KillAllPlayers();

        List<PlayerConfigurations> playersToSpawn = new();

        // If game not started from start screen (development), create some players
        if (JoinScreenManager.Instance == null || JoinScreenManager.Instance.playerConfigs == null || JoinScreenManager.Instance.playerConfigs.Count == 0)
        {
            //Determine amount of players
            int _playerSpawnAmount = 0;
            if (createAPlayerForEachGamepad) {
                _playerSpawnAmount = Gamepad.all.Count; 
            }
            else {  
                _playerSpawnAmount = playerCountToSpawn;
            }
            Debug.Log($"Game not started via proper method, constructing {_playerSpawnAmount} players.");

            // Spawn
            for (int i = 0; i < _playerSpawnAmount; i++) {
                InputDevice playerInputDevice = null;

                // If creating a player for each gamepad, determing controlscheme is easy
                if (createAPlayerForEachGamepad) {
                    playerInputDevice = Gamepad.all[i];
                }
                else {
                    // If not, ensure some level of control.
                    if(i < Gamepad.all.Count) {
                        playerInputDevice = Gamepad.all[i];
                    }
                    else if(Gamepad.all.Count != 0) {
                        Debug.LogError("Not enough Gamepads found, repeating last gamepad for input.");
                        playerInputDevice = Gamepad.all[^1];
                    }
                    else {
                        Debug.LogError("No Gamepad found, unexpected behaivor expected.");
                    }
                }

                // Spawn player
                PlayerInput playerInputObj = SpawnAPlayer(i, playerInputDevice);

                // Move player to a spawnpoint
                TeleportPlayerToSpawn(i, playerInputObj.gameObject);

                // Verify Component Validity
                VerifyPlayer(playerInputObj.gameObject, i);
            }

            // Raise Event for spawned players
            OnAllPlayersSpawned?.Invoke();

            // Development players created, exit function

            return;
        }

        // Spawning apporpriatly
        playersToSpawn = JoinScreenManager.Instance.playerConfigs;
        for(int i = 0; i < playersToSpawn.Count;i++) {

            // Instansiate
            PlayerInput playerInputObj = SpawnAPlayer(playersToSpawn[i].playerIndex, playersToSpawn[i].input.devices[0]);

            // Move player to a spawnpoint
            TeleportPlayerToSpawn(playersToSpawn[i].playerIndex, playerInputObj.gameObject);

            // Verify Component Validity
            VerifyPlayer(playerInputObj.gameObject, playersToSpawn[i].playerIndex, true);     
        }


        // Raise Event for spawned players
        OnAllPlayersSpawned?.Invoke();
    }

    private PlayerInput SpawnAPlayer(int playerIndex, InputDevice inputDevice)
    {
        return GameStartManager.SpawnAPlayer(playerPrefab, playerIndex, inputDevice);
    }

    public  static PlayerInput SpawnAPlayer(GameObject playerPrefab, int playerIndex, InputDevice inputDevice)
    {
        PlayerInput playerInputObj = PlayerInput.Instantiate(prefab: playerPrefab,
                                                        playerIndex: playerIndex,
                                                        controlScheme: null,
                                                        splitScreenIndex: -1,
                                                        pairWithDevice: inputDevice
                                                        );

        playerInputObj.transform.SetParent(null);

        return playerInputObj;
    }

    public static void KillAllPlayers()
    {
        var players = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
        List<Transform> test = new List<Transform>();

        foreach (var item in players)
        {
            Destroy(item.gameObject);
        }
    }

    private void TeleportPlayerToSpawn(int playerIndex, GameObject player)
    {
        // Get Sapwn location
        Vector3 spawnAt = transform.position;
        try
        {
            spawnAt = spawnPoints[playerIndex].position;
        }
        catch
        {
            Debug.LogError("Appropriate spawnpoint not found, spawning on GameStartManager");
        }

        player.transform.position = spawnAt;
    }

    public static void VerifyPlayer(GameObject playerToVerify, int playerIndex, bool appropriatlySpawned = false)
    {
        PlayerStats playerStats = playerToVerify.GetComponent<PlayerStats>();
        Movement playerMovement = playerToVerify.GetComponent<Movement>();
        Aiming playerAim = playerToVerify.GetComponent<Aiming>();
        WeaponUser weaponUser = playerToVerify.GetComponent<WeaponUser>();
        Jump jump = playerToVerify.GetComponent<Jump>();
        ReadyBehaivor  readyBehaivor = playerToVerify.GetComponent<ReadyBehaivor>();
        PlayerAnimationManager playerAnimationManager = playerToVerify.GetComponent<PlayerAnimationManager>();
        GroundChecker groundChecker = playerToVerify.GetComponent<GroundChecker>();
        CharacterSelectManager characterSelectManager = playerToVerify.GetComponent<CharacterSelectManager>();


        // Stats set-up
        if (playerStats == null)
        {
            Debug.LogError("Player prefab missing PlayerStats script");
        }
        else
        {
            playerStats.playerIndex = playerIndex;
        }

        // Movement setup
        if (playerMovement == null)
        {
            Debug.LogError("Player prefab missing movement script");
        }
        else
        {
            playerMovement.appropriatlySpawned = appropriatlySpawned;
        }

        // Aiming setup
        if (playerAim == null)
        {
            Debug.LogError("Player prefab missing aiming script");
        }
        else
        {
            playerAim.appropriatlySpawned = appropriatlySpawned;
        }

        // Weapon Usage setup
        if (weaponUser == null)
        {
            Debug.LogError("Player prefab missing Weapon User script");
        }
        else
        {
            weaponUser.appropriatlySpawned = appropriatlySpawned;
            weaponUser.userIndex = playerIndex;
        }

        // Jump setup
        if(jump == null)
        {
            Debug.LogError("Player prefab missing Jump script");
        }
        else
        {
            jump.appropriatlySpawned = appropriatlySpawned;
        }

        // Ready behaivor
        if(readyBehaivor == null)
        {
            Debug.LogError("Player prefab missing ReadyBehaivor script");
        }

        // Ground checker
        if (groundChecker == null)
        {
            Debug.LogError("Player prefab missing GroundChecker script");
        }

        // Ready behaivor
        if (playerAnimationManager == null)
        {
            Debug.LogError("Player prefab missing PlayerAnimationManager script");
        }

        // Character select manager
        if(characterSelectManager == null)
        {
            Debug.LogError("Player prefab missing Character Select Manager script");
        }
        else
        {
            characterSelectManager.currentIndex = playerIndex;
        }
    }

    private void StartCameraTracking()
    {
        if (CameraController.Instance == null) return;

        var players = FindObjectsOfType<PlayerStats>().ToList();
        List<Transform> playerTransforms = new List<Transform>();
        foreach (var t in players)
        {
            playerTransforms.Add(t.transform);
        }

        if(playerTransforms.Count < 1) { return; }
        CameraController.Instance.StartTrackingObjects(playerTransforms);
    }
}

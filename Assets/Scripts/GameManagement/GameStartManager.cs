using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStartManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPoints;

    [Header("Development options")]
    [SerializeField] private bool createAPlayerForEachGamepad = false;
    [SerializeField] [Range(0,4)] private int playerCountToSpawn = 2;

    private void Start()
    {
        SpawnPlayers();
    }


    private void SpawnPlayers()
    {
        if (playerPrefab == null) return;

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

                // Spawn player and determine spawn position
                PlayerInput playerInputObj = PlayerInput.Instantiate(
                                                        prefab: playerPrefab,
                                                        playerIndex: i,
                                                        controlScheme: null,
                                                        splitScreenIndex: -1,
                                                        pairWithDevice: playerInputDevice
                                                        );

                // Move player to a spawnpoint
                TeleportPlayerToSpawn(i, playerInputObj.gameObject);

                // Verify Component Validity
                VerifyPlayer(playerInputObj.gameObject, i);
            }

            // Development players created, exit function
            return;
        }

        // Spawning apporpriatly
        playersToSpawn = JoinScreenManager.Instance.playerConfigs;
        for(int i = 0; i < playersToSpawn.Count;i++) { 

            // Instansiate
            PlayerInput playerInputObj = PlayerInput.Instantiate(   prefab: playerPrefab, 
                                                                    playerIndex: playersToSpawn[i].playerIndex, 
                                                                    controlScheme: null, 
                                                                    splitScreenIndex: -1, 
                                                                    pairWithDevice: playersToSpawn[i].input.devices[0]
                                                                    );


            // Move player to a spawnpoint
            TeleportPlayerToSpawn(playersToSpawn[i].playerIndex, playerInputObj.gameObject);

            // Verify Component Validity
            VerifyPlayer(playerInputObj.gameObject, playersToSpawn[i].playerIndex, true);     
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

    private void VerifyPlayer(GameObject playerToVerify, int playerIndex, bool appropriatlySpawned = false)
    {
        PlayerStats playerStats = playerToVerify.GetComponent<PlayerStats>();
        Movement playerMovement = playerToVerify.GetComponent<Movement>();
        Aiming playerAim = playerToVerify.GetComponent<Aiming>();
        WeaponUser weaponUser = playerToVerify.GetComponent<WeaponUser>();


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
        }
    }
}

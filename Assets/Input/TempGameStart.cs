using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TempGameStart : MonoBehaviour
{
    public GameObject playerPrefab;
    public Transform startPos;

    private void Start()
    {
        List<PlayerInput> players = TempSpawnController.playersJoined;
        PlayerInputManager pim = GetComponent<PlayerInputManager>();

        for (int i = 0; i < 2/*players.Count*/; i++)
        {
            Vector3 spawnPos = startPos.position;
            spawnPos.x += 1 * i;
            PlayerInput.Instantiate(playerPrefab, i, "Gameplay", -1, Gamepad.all[i]);
            //GameObject playerObj = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
           /* playerObj.name = "Player: " + (i + 1).ToString();
            PlayerInput playerInput = playerObj.GetComponent<PlayerInput>();*/
        }
    }
}

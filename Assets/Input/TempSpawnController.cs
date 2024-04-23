using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class TempSpawnController : MonoBehaviour
{
    public static List<PlayerInput> playersJoined = new();

    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            SceneManager.LoadScene("DevRonjaPersistanceCheck");
        }
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        playersJoined.Add(input);
    }
}

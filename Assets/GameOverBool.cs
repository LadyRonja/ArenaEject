using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverBool : MonoBehaviour
{
    public static bool gameOver = false;

    private void Awake()
    {
        SceneManager.sceneLoaded += ResetGameOverFlag;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= ResetGameOverFlag;
    }
    
    public void ResetGameOverFlag(Scene scene, LoadSceneMode mode)
    {
        gameOver = false;
    }
}

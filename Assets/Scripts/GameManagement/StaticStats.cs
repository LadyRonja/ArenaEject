using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticStats : MonoBehaviour
{
    public static Dictionary<int, int> playerWins = new Dictionary<int, int>();

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
        EndGameManager.Instance.gameIsOver = false;
    }
}

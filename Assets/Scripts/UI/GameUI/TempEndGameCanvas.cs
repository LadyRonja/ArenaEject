using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempEndGameCanvas : MonoBehaviour
{
    public Transform playerGridRegion;
    public Selectable defaultButtonChoice;
    
    public GameObject BG;
    public GameObject gameOverText;
    public GameObject menuButton;
    public GameObject againButton;

    private void Start()
    {
        /*if(defaultButtonChoice!= null)
        {
            defaultButtonChoice.Select();
        }*/
    }


    public void GoToNextScene()
    {
        EndGameManager.Instance.GoToNextScene();
    }

    public void GoToMain()
    {
        SceneHandler.Instance.GoToScene(Paths.START_SCENE_NAME);
    }
}

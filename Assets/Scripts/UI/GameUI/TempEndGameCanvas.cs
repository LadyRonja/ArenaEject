using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TempEndGameCanvas : MonoBehaviour
{
    public Transform playerGridRegion;
    public Selectable defaultButtonChoice;

    private void Start()
    {
        if(defaultButtonChoice!= null)
        {
            defaultButtonChoice.Select();
        }
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

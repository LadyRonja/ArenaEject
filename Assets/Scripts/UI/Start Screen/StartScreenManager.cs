using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{

    private static StartScreenManager instance;
    public static StartScreenManager Instance { get => instance; }

    [SerializeField] private GameObject joinScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    private List<GameObject> screens;


    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            screens = new List<GameObject>();
            if(joinScreen != null) screens.Add(joinScreen);
            if (optionsScreen != null) screens.Add(optionsScreen);
            if (creditsScreen != null) screens.Add(creditsScreen);

            TurnOffAllSCreens();
        }
        else
        {

            Destroy(this.gameObject);
        }
    }

    public void GoToJoin()
    {
        GoToScreen(joinScreen);
    }

    public void GoToOptions()
    {
        GoToScreen(optionsScreen);
    }
    public void GoToCredits()
    {
        GoToScreen(creditsScreen);
    }

    private void GoToScreen(GameObject screen)
    {
        TurnOffAllSCreens(screen);
        if (screen != null) screen.SetActive(!screen.activeSelf);
    }

    public void GoToDefault()
    {
        TurnOffAllSCreens();
    }

    private void TurnOffAllSCreens()
    {
        foreach (GameObject s in screens)
        {
            s.SetActive(false);
        }
    }

    private void TurnOffAllSCreens(GameObject exclude)
    {
        foreach (GameObject s in screens)
        {
            if(s != exclude)
            {
                s.SetActive(false);
            }
        }
    }

    public void StartGame()
    {
        //Dictionary<int, int> pci = JoinScreenManager.Instance.PlayerToControllerIndex;
        //PlayerData.playerToControllerBinding = pci;

        SceneManager.LoadScene(Paths.FARRAZ_SCENE_NAME);
    }
}

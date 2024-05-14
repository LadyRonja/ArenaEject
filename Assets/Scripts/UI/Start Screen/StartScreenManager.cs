using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenManager : MonoBehaviour
{

    private static StartScreenManager instance;
    public static StartScreenManager Instance { get => instance; }

    [SerializeField] private GameObject buttonRegion;
    [SerializeField] private GameObject joinScreen;
    [SerializeField] private GameObject optionsScreen;
    [SerializeField] private GameObject creditsScreen;
    private List<GameObject> screens;

    [SerializeField] private GameObject joinScreenManagerPrefab;


    private void Awake()
    {
        if (instance == null || instance == this)
        {
            instance = this;
            screens = new List<GameObject>();
            if(joinScreen != null) screens.Add(joinScreen);
            if (optionsScreen != null) screens.Add(optionsScreen);
            if (creditsScreen != null) screens.Add(creditsScreen);

            if(JoinScreenManager.Instance == null)
            {
                Instantiate(joinScreenManagerPrefab);
            }

            PlayerInputManager.instance.DisableJoining(); 
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
        PlayerInputManager.instance.EnableJoining();
        buttonRegion.SetActive(false);
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
        buttonRegion.SetActive(true);
        try
        {
            buttonRegion.transform.GetChild(0).gameObject.GetComponent<Button>().Select();
            JoinScreenManager.Instance.OnSceneLoad();
        }
        catch 
        {

        }
        TurnOffAllSCreens();
    }

    private void TurnOffAllSCreens()
    {
        PlayerInputManager.instance.DisableJoining();
        foreach (GameObject s in screens)
        {
            s.SetActive(false);
        }
    }

    private void TurnOffAllSCreens(GameObject exclude)
    {
        PlayerInputManager.instance.DisableJoining();
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
        SceneManager.LoadScene(Paths.FARRAZ_SCENE_NAME);
    }
}

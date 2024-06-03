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
    [Space]
    [SerializeField] private GameObject bg1;
    [SerializeField] private GameObject bg2;

    [Space]
    [SerializeField] private Button joinButton;
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button quitButton;
    [Space]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button creditsBackButton;
    private Button lastSelected;

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
            if(joinButton != null)
            {
                lastSelected = joinButton;
            }
        }
        else
        {

            Destroy(this.gameObject);
        }
    }

    public void GoToJoin()
    {
        GoToScreen(joinScreen);
        if (bg1 != null) bg1.SetActive(false);
        if (bg2 != null) bg2.SetActive(false);
        PlayerInputManager.instance.EnableJoining();
        buttonRegion.SetActive(false);
        if (joinButton != null)
        {
            lastSelected = joinButton;
        }
    }

    public void GoToOptions()
    {
        GoToScreen(optionsScreen);
        if(musicSlider != null)
        {
            musicSlider.Select();
        }
        if(optionsButton != null)
        {
            lastSelected = optionsButton;
        }
    }
    public void GoToCredits()
    {
        GoToScreen(creditsScreen);
        musicSlider.Select();
        if(creditsBackButton != null)
        {
            creditsBackButton.Select();
        }
        if (creditsButton != null)
        {
            lastSelected = creditsButton;
        }
    }

    private void GoToScreen(GameObject screen)
    {
        TurnOffAllSCreens(screen);
        if(bg1!= null) bg1.SetActive(true);
        if (bg2 != null) bg2.SetActive(true);
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
        if(lastSelected != null)
        {
            lastSelected.Select();
        }
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
        //SceneManager.LoadScene(Paths.FARRAZ_SCENE_NAME);
        SceneHandler.Instance.GoToScene(Paths.FARRAZ_SCENE_NAME);
    }
}

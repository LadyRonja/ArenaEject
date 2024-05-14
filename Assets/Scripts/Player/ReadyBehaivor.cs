using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ReadyBehaivor : MonoBehaviour
{
    [HideInInspector] public bool inExitRegion = false;
    [HideInInspector] public bool inReadyArea = false;
    [HideInInspector] public bool isReady = false;

    [SerializeField] private GameObject readyCanvas;
    [SerializeField] private Image displayImage;
    [SerializeField] private Sprite readySprite;
    [SerializeField] private Sprite exitSprite;

    private void Start()
    {
        if(readyCanvas != null)
        {
            HideDisplayCanvas();
        }
        else
        {
            Debug.LogError("Player ReadyBehaivor Missing reference to readycanvas - disabling component");
            this.enabled = false;
        }
    }

    private void OnStart(InputValue value)
    {
        if(inReadyArea && !isReady)
        {
            isReady = true;
            DisplayReady();
        }

        if(inExitRegion) {
            // TODO: Let player index 0 reset the lobby, but have all other just leave the lobby
            /*
            if(TryGetComponent<PlayerStats>(out PlayerStats myStats))
            {
                if(myStats.playerIndex == 0)
                {
                    StartScreenManager.Instance.GoToDefault();
                }
                else
                {
                    JoinScreenManager.Instance.RemovePlayerFromLobby(myStats.playerIndex);
                }
                return;
            }*/

            StartScreenManager.Instance.GoToDefault();
        }
    }

    public void DisplayReady()
    {
        if (readyCanvas != null)
        {
            try
            {
                displayImage.sprite= readySprite;
            }
            catch 
            {

            }
            readyCanvas.SetActive(true);
        }
    }

    public void DisplayExit()
    {
        if (readyCanvas != null)
        {
            try
            {
                displayImage.sprite = exitSprite;
            }
            catch
            {

            }
            readyCanvas.SetActive(true);
        }
    }

    public void HideDisplayCanvas()
    {
        if (readyCanvas != null)
        {
            readyCanvas.SetActive(false);
        }
    }
}

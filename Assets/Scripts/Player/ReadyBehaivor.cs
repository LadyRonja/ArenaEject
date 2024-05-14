using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ReadyBehaivor : MonoBehaviour
{
    [HideInInspector] public bool inReadyArea = false;
    [HideInInspector] public bool isReady = false;

    [SerializeField] private GameObject readyCanvas;

    private void Start()
    {
        if(readyCanvas != null)
        {
            HideReady();
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
    }

    public void DisplayReady()
    {
        if (readyCanvas != null)
        {
            readyCanvas.SetActive(true);
        }
    }

    public void HideReady()
    {
        if (readyCanvas != null)
        {
            readyCanvas.SetActive(false);
        }
    }
}

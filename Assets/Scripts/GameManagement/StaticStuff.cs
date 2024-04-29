using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticStuff : MonoBehaviour
{
    public static StaticStuff Instance { get; private set; }

    public JoinScreenManager JoinScreenManager { get; set; }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        new GameObject("StaticStuff", typeof(StaticStuff));
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

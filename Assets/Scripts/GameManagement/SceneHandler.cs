using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneHandler : MonoBehaviour
{
    private static SceneHandler instance;
    public static SceneHandler Instance { get { return GetInstance(); } }

    private GameObject transitionInstance;
    private bool changingScene = false;

    private SerializedAnimationCurve scaleCurve;


    private void Awake()
    {
        if(instance == null || instance == this)
        {
            if(instance != this)
            {
                SceneManager.sceneLoaded += delegate { Detransition(); };
            }

            instance = this;
            EnsureTransitionInstance();
            DontDestroyOnLoad(gameObject);
            
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= delegate { Detransition(); };
    }

    public void GoToScene(string sceneToGoTo)
    {
        if (changingScene) { return; }
        changingScene = true;
        StopAllCoroutines();
        StartCoroutine(Transition(sceneToGoTo, true));
    }

    public void GoToScene(int sceneIndex)
    {
        // Get path to scene
        string scenePath = SceneUtility.GetScenePathByBuildIndex(sceneIndex);

        // Seperate out the name from the path
        int sceneNameStart = scenePath.LastIndexOf("/", StringComparison.Ordinal) + 1;
        int sceneNameEnd = scenePath.LastIndexOf(".", StringComparison.Ordinal);
        int sceneNameLength = sceneNameEnd - sceneNameStart;
        string sceneName = scenePath.Substring(sceneNameStart, sceneNameLength);

        GoToScene(sceneName);
    }

    private void Detransition()
    {
        StopAllCoroutines();
        StartCoroutine(Transition("", false));
    }

    private IEnumerator Transition(string sceneToTransitionTo, bool transitionOut)
    {
        EnsureTransitionInstance();
        Transform transitionTransform = transitionInstance.transform.GetChild(0);
        /*if(transitionOut)
        {
            transitionTransform.localScale = Vector3.zero;
        }
        else
        {
            transitionTransform.localScale = Vector3.one * scaleRequiredToCover;
        }

        float timeToTransition = transitionTime;
        float timePassed = 0f;

        Vector3 startScale = transitionTransform.localScale;
        Vector3 endScale = Vector3.zero;
        if (transitionOut)
        {
            endScale = Vector3.one * scaleRequiredToCover;
        }

        while (timePassed < timeToTransition)
        {
            transitionTransform.localScale = Vector3.Lerp(startScale, endScale, ( timePassed / timeToTransition));

            timePassed+= Time.deltaTime;
            yield return null;
        }

        transitionTransform.localScale = endScale;*/

        #region Attempting use of animation curve
        EnsureAnimationCurve();

        float duration = scaleCurve.curve.length;
        float timePassed = 0f;
        if (transitionOut)
        {
            while (timePassed < duration)
            {
                float scaleAtTime = scaleCurve.curve.Evaluate(timePassed);
                transitionTransform.localScale = Vector3.one * scaleAtTime;
                timePassed += Time.deltaTime;
                yield return null;
            }
            transitionTransform.localScale = Vector3.one * scaleCurve.curve.Evaluate(duration);
        }
        else
        {
            float timeLeft = duration;
            while (timeLeft > 0)
            {
                float scaleAtTime = scaleCurve.curve.Evaluate(timeLeft);
                transitionTransform.localScale = Vector3.one * scaleAtTime;
                timeLeft -= Time.deltaTime;
                yield return null;
            }
            transitionTransform.localScale = Vector3.zero;
        }

        #endregion

        changingScene = false;
        if(transitionOut)
        {
            SceneManager.LoadSceneAsync(sceneToTransitionTo);
        }
        yield return null;
    }


    private void EnsureTransitionInstance()
    {
        if(transitionInstance != null) { return; }


        GameObject transitionCanvasObject = new GameObject("Transition Canvas");

        Canvas transitionCanvas = transitionCanvasObject.AddComponent<Canvas>();
        transitionCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
        transitionCanvas.sortingOrder = 10;

        GameObject transitionImageObject = new GameObject("Transition Image");
        transitionImageObject.transform.SetParent(transitionCanvasObject.transform);

        RectTransform transitionImageTransform = transitionImageObject.AddComponent<RectTransform>();
        transitionImageTransform.pivot = new Vector2(0.5f, 0.5f);

        Image transitionImage = transitionImageObject.AddComponent<Image>();
        Sprite transitionSprite = Resources.Load<Sprite>(Paths.TRANSITION_SPRITE);
        if(transitionSprite != null)
        {
            float spriteWidth = transitionSprite.rect.width;
            float spriteHeight = transitionSprite.rect.height;
            transitionImageTransform.sizeDelta = new Vector2(spriteWidth, spriteHeight);
            transitionImage.sprite = transitionSprite;
        }
        else
        {
            Debug.LogError("Transition Sprite Missing from Resources");
            transitionImage.color = Color.magenta;
        }

        transitionImageTransform.localPosition = Vector3.zero;
        transitionImageTransform.localScale = Vector3.zero;

        transitionInstance = transitionCanvasObject;
        DontDestroyOnLoad(transitionInstance);
    }

    private void EnsureAnimationCurve()
    {
        if(scaleCurve != null)
        {
            EnsureCurve();
            
            return;
        }

        scaleCurve = Resources.Load<SerializedAnimationCurve>(Paths.TRANSITION_CURVE);
        if(scaleCurve != null)
        {
            EnsureCurve();
            
            return;
        }

        scaleCurve = new SerializedAnimationCurve();
        EnsureCurve();


        void EnsureCurve()
        {
            //Debug.Log("ScaleCurve.curve.length: " + scaleCurve.curve.length);
            if (scaleCurve.curve == null || scaleCurve.curve.length == 0)
            {
                scaleCurve.curve = new AnimationCurve();
                scaleCurve.curve.AddKey(0f, 0f);
                scaleCurve.curve.AddKey(3f, 15f);
            }
        }
    }

    private static SceneHandler GetInstance() {
        if (instance != null)
        {
            return instance; 
        }

        SceneHandler newInstance = new GameObject().AddComponent<SceneHandler>();
        newInstance.transform.name = "Scene Handler";
        newInstance = instance;
        return newInstance;
    }
}

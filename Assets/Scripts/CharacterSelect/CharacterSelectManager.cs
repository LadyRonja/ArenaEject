using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using static UnityEngine.InputSystem.InputSettings;

public class CharacterSelectManager : MonoBehaviour
{
    private bool ableToChange { get => GetCanUpdate(); }
    [SerializeField] private Transform modelParent;
    [SerializeField] private PlayerAnimationManager pam;
    [SerializeField] private PlayerStats ps;
    [HideInInspector] public int currentIndex = -1;

    [HideInInspector] public CharacterSelectStation closeStation;
    [SerializeField] private List<GameObject> modelPrefabs = new();

    public void Start()
    {
        if(!TryGetComponent<PlayerAnimationManager>(out pam))
        {
            Debug.LogError("CharacterSelectMaanger unable to find PlayerAnimationManager");
        }

        if (!TryGetComponent<PlayerStats>(out ps))
        {
            Debug.LogError("CharacterSelectMaanger unable to find PlayerStats");
        }
    }

    private void OnDPadRight(InputValue value)
    {
        if(!ableToChange) { return; }
        ChangeModel(true);
    }

    private void OnDPadLeft(InputValue value)
    {
        if (!ableToChange) { return; }
        ChangeModel(false);
    }

    private void ChangeModel(bool goingRight)
    {
        if(modelParent == null) { return; }
        // Models were planned to be player unique.
        // Since that won't happen, we don't need an outside manger to deal with it.
        //GameObject newModel = closeStation.GetNextModel(goingRight, currentIndex, this);

        GameObject newModel = GetNextModel(goingRight, currentIndex, this);

        // If updatemodel fails, return
        if (!UpdateModel(newModel))
        {
            return;
        }

        UpdateModelInfoForSceneChanges();

        //Debug.Log("New model");
    }


    private GameObject GetNextModel(bool goingRight, int startIndex, CharacterSelectManager caller)
    {
        if (modelPrefabs.Count < 1) { return null; }

        int nextIndex = startIndex;
        if (goingRight)
        {
            nextIndex++;
        }
        else
        {
            nextIndex--;
        }

        if (nextIndex < 0) { nextIndex = modelPrefabs.Count - 1; }
        if (nextIndex >= modelPrefabs.Count) { nextIndex = 0; }

        //Debug.Log("Returning a model");
        GameObject objToReturn = Instantiate(modelPrefabs[nextIndex]);
        caller.currentIndex = nextIndex;
        return objToReturn;
    }

    public void UpdateModel(int modelIndex)
    {
        GameObject newModelObj = Instantiate(modelPrefabs[modelIndex], transform.position, Quaternion.identity);
        Debug.Log(newModelObj.name);
        UpdateModel(newModelObj);
    }

    private bool UpdateModel(GameObject newModel)
    {
        if (newModel == null) { return false; }
        Animator newAnim = newModel.GetComponentInChildren<Animator>();
        if (newAnim == null)
        {
            Debug.LogError("New Model missing Animator Component");
            Destroy(newModel);
            return false;
        }

        foreach (Transform child in modelParent)
        {
            DestroyImmediate(child.gameObject);
        }

        List<SkinnedMeshRenderer> newRenderes = new();
        foreach (Transform child in newModel.transform)
        {
            List<SkinnedMeshRenderer> foundRenderers = child.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
            newRenderes.AddRange(foundRenderers);
        }
        foreach (SkinnedMeshRenderer renderer in newRenderes)
        {
            try
            {
                renderer.material = Instantiate(renderer.material);
                renderer.material.color = ps.colors[ps.playerIndex];
            }
            catch
            {

            }
        }
        ps.myRenderers = newRenderes;

        if(SceneManager.GetActiveScene().name == Paths.START_SCENE_NAME)
        {
            CameraController.Instance.AddShake();
        }
        newModel.transform.SetParent(modelParent.transform);
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localRotation = Quaternion.identity;

        pam.animator = newAnim;
        Debug.Log("New model sucsesfully updated");
        return true;
    }

    private void UpdateModelInfoForSceneChanges()
    {
        JoinScreenManager.Instance.UpdatePlayerConfigModel(ps.playerIndex, currentIndex);
    }

    

    private bool GetCanUpdate()
    {
        if (closeStation == null) { return false; }
        if (pam == null) { return false; }
        if (ps == null) { return false; }

        return true; 
    }
}

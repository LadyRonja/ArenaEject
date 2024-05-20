using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterSelectManager : MonoBehaviour
{
    private bool ableToChange { get => GetCanUpdate(); }
    [SerializeField] private Transform modelParent;
    [SerializeField] private PlayerAnimationManager pam;
    [SerializeField] private PlayerStats ps;
    [HideInInspector] public int currentIndex = -1;

    [HideInInspector] public CharacterSelectStation closeStation;

    private void Start()
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
        GameObject newModel = closeStation.GetNextModel(goingRight, currentIndex, this);
        if (newModel == null) { return; }
        Animator newAnim = newModel.GetComponentInChildren<Animator>();
        if (newAnim == null) { 
            Debug.Log("New Model missing Animator Component");
            Destroy(newModel);
            return;
        }

        foreach (Transform child in modelParent)
        {
            DestroyImmediate(child.gameObject);
        }

        List<SkinnedMeshRenderer> newRenderes = new();
        foreach (Transform child in newModel.transform)
        {
            List < SkinnedMeshRenderer > foundRenderers = child.GetComponentsInChildren<SkinnedMeshRenderer>().ToList();
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

        CameraController.Instance.AddShake();
        newModel.transform.SetParent(modelParent.transform);
        newModel.transform.localPosition = Vector3.zero;
        newModel.transform.localRotation = Quaternion.identity;

        pam.animator = newAnim;

        Debug.Log("New model");
    }

    private bool GetCanUpdate()
    {
        if (closeStation == null) { return false; }
        if (pam == null) { return false; }
        if (ps == null) { return false; }

        return true; 
    }
}

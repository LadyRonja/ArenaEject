using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
public struct ModelData
{
    public ModelData(GameObject model, int index)
    {

    }

    GameObject model;
    int index;
}*/

public class CharacterSelectStation : MonoBehaviour
{
    //[SerializeField] private List<GameObject> modelPrefabs = new();

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<CharacterSelectManager>(out CharacterSelectManager csm))
        {
            csm.closeStation = this;
           //Debug.Log("Character in range to change");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<CharacterSelectManager>(out CharacterSelectManager csm))
        {
            csm.closeStation = null;
            //Debug.Log("Character no longer range to change");
        }
    }

    /*public GameObject GetNextModel(bool goingRight, int startIndex, CharacterSelectManager caller)
    {
        if (modelPrefabs.Count < 1) { return null; }

        int nextIndex = startIndex;
        if(goingRight) { 
            nextIndex++;
        }
        else {
            nextIndex--;
        }

        if(nextIndex < 0) { nextIndex = modelPrefabs.Count - 1; }
        if(nextIndex >= modelPrefabs.Count) { nextIndex = 0; }

        Debug.Log("Returning a model");
        GameObject objToReturn = Instantiate(modelPrefabs[nextIndex]);
        caller.currentIndex = nextIndex;
        return objToReturn;
    }*/
}

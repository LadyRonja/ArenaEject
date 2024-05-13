using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerIndex = 0;
    public int lives = 1;
    public int timeAlive = 0;
    
    public bool alive { get => lives > 0; }

    [Header("TEMP")]
    public List<Color> colors = new List<Color>();
    public SkinnedMeshRenderer myRenderer;

    private void Start()
    {
        if(myRenderer != null)
        {
            if (colors.Count != 4) return;

            myRenderer.material = Instantiate(myRenderer.material);
            myRenderer.material.color = colors[playerIndex];
        }
    }

    /*private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("KillPlane"))
        {
            Debug.Log("See comment beneth this debug");
            // The killplane should probably be the one to handle this, I'm leaving it in for now.
            // For example if we're on a level where you don't destroy the player.
            // I'm leaving it here for now but this might need to be removed
            Destroy(gameObject);
        }
    }*/
}

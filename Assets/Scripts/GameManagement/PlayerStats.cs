using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerIndex = 0;
    public int lives = 1;
    public int timeAlive = 0;
    public float finalKnockbackDisplay { get; set; }
    
    public bool alive { get => lives > 0; }

    [Header("TEMP")]
    public List<Color> colors = new List<Color>();
    public List<Sprite> playerSprites = new();
    public List<Sprite> endGamePlayerSprites = new();
    public Sprite backGrpundSprites;
    public List<SkinnedMeshRenderer> myRenderers = new();

    private void Start()
    {
        if(myRenderers != null)
        {
            if (colors.Count != 4) return;

            foreach (SkinnedMeshRenderer rendered in myRenderers)
            {
                rendered.material = Instantiate(rendered.material);
                rendered.material.color = colors[playerIndex];
            }
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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int playerIndex = 1;
    public int lives = 3;
    public bool alive { get => lives > 0; }

    [Header("TEMP")]
    public List<Color> colors = new List<Color>();
    public MeshRenderer myRenderer;

    private void Start()
    {
        if(myRenderer != null)
        {
            if (colors.Count != 4) return;

            myRenderer.material = Instantiate(myRenderer.material);
            myRenderer.material.color = colors[playerIndex-1];
        }
    }
}

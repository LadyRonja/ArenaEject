using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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
    public List<Material> aimMaterials = new List<Material>();
    public DecalProjector aimAssisst;
    public List<Material> throwMaterials = new List<Material>();
    public DecalProjector throwAssisst;

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

        if(aimAssisst != null)
        {
            try
            {
                aimAssisst.material = Instantiate(aimMaterials[playerIndex]);
            }
            catch
            {

                throw;
            }
        }

        if(throwAssisst != null)
        {
            try
            {
                throwAssisst.material = Instantiate(throwMaterials[playerIndex]);
            }
            catch
            {

                throw;
            }
        }
    }

    
}

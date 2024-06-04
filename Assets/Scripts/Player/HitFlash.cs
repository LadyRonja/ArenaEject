using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    [SerializeField] private bool flashPlayerColor = true;
    [SerializeField] private Color colorToFlash = Color.white;
    [SerializeField] private float flashDuration = 0.28f;

    private bool CanFlash { get { return ((ComponentsVerified() && !isFlashing) == true); } }
    private List<SkinnedMeshRenderer> Renderes { get { return GetRenderes(); } }

    private PlayerStats _playerStats;
    private KnockBackHandler _knockbackHandler;
    private bool isFlashing = false;

    private void Start()
    {
        SetUp();

        if(flashPlayerColor)
        {
            if (TryGetComponent<PlayerStats>(out PlayerStats ps))
            {
                colorToFlash = ps.colors[ps.playerIndex];
            }
        }

    }

    private void SetUp()
    {
        // Find PLayerStats and instansiate materials
        if (!TryGetComponent<PlayerStats>(out _playerStats))
        {
            Debug.LogError("HitFlash Cannot find PlayerStats component");
        }
        else
        {
            List<SkinnedMeshRenderer> smrs = GetRenderes();
            foreach (SkinnedMeshRenderer renderer in smrs)
            {
                renderer.material = Material.Instantiate(renderer.material);
            }
        }

        if (!TryGetComponent<KnockBackHandler>(out _knockbackHandler))
        {
            Debug.LogError("HitFlash Cannot find KnockBackHandler component");
        }
        else
        {
            if (CanFlash)
            {
                _knockbackHandler.OnKnockbackRecieved += CauseHitFlash;
            }
        }
    }

    private void OnDestroy()
    {
        if(ComponentsVerified())
        {
            _knockbackHandler.OnKnockbackRecieved -= CauseHitFlash;
        }
    }

    private void OnDisable()
    {
        OnDestroy();
    }

    private void CauseHitFlash()
    {
        if (!CanFlash) { return; }
        isFlashing = true;
        StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        List<SkinnedMeshRenderer> smrs = GetRenderes();
        List<Color> endColors = new();
        Color startColor = colorToFlash;
        for (int i = 0; i < smrs.Count; i++)
        {
            endColors.Add(smrs[i].material.color);
            smrs[i].material.color = startColor;
        }

        float timePassed = 0f;

        while (timePassed < flashDuration)
        {
            for (int i = 0; i < smrs.Count; i++)
            {
                smrs[i].material.color = Color.Lerp(startColor, endColors[i], (timePassed / flashDuration));
            }
            timePassed += Time.deltaTime;
            yield return null;
        }

        for (int i = 0; i < smrs.Count; i++)
        {
            smrs[i].material.color = endColors[i];
        }
        isFlashing = false;
        yield return null;
    }

    private bool ComponentsVerified()
    {
        if (_playerStats == null) { return false; }

        return true;
    }

    private List<SkinnedMeshRenderer> GetRenderes()
    {
        List<SkinnedMeshRenderer> output = new();
        if (_playerStats == null) { return output; }

        output = _playerStats.myRenderers;

        if(output == null) { 
            output = new(); }

        return output;
    }
}

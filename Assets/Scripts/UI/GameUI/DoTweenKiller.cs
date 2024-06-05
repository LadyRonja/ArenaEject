using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoTweenKiller : MonoBehaviour
{
    public void OnDisable()
    {
        transform.DOKill();
    }
    public void OnDestroy()
    {
        transform.DOKill();
    }
}

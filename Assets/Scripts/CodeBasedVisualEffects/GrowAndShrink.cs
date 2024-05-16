using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowAndShrink : MonoBehaviour
{
    [SerializeField] float initDelay = 0.3f;
    [SerializeField] float timeToFullSize = 0.5f;
    [SerializeField] float fullSizeLinger = 1f;
    [SerializeField] float timeToShrink = 1f;

    private void Start()
    {
        transform.localScale = Vector3.zero;
        StartCoroutine(ChangeSize(Vector3.zero, Vector3.one, timeToFullSize, initDelay));
        StartCoroutine(ChangeSize(Vector3.one, Vector3.zero, timeToShrink, initDelay + timeToFullSize + fullSizeLinger));
        Destroy(gameObject, initDelay + timeToFullSize + timeToShrink + fullSizeLinger);
    }

    private void OnDestroy()
    {
        this.StopAllCoroutines();
    }

    private IEnumerator ChangeSize(Vector3 startSize, Vector3 endSize, float inTime, float initialDelay)
    {
        yield return new WaitForSeconds(initialDelay);
        float timePassed = 0;

        while (timePassed < inTime)
        {
            transform.localScale = Vector3.Lerp(startSize, endSize, (timePassed / inTime));

            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endSize;

        yield return null;
    }
}

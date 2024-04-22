using System;
using System.Collections;
using TMPro.Examples;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 3000f;
    [SerializeField] private float duration = 0.1f;

    private void Start()
    {
        StartCoroutine(Explode());
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent<KnockBackHandler>(out KnockBackHandler kbh))
        {
            Vector3 dir = other.transform.position - transform.position;
            dir.y = 0;
            dir.Normalize();
            kbh.GetKnockedBack(dir, explosionForce);
        }
    }

    private IEnumerator Explode() {

        float timePassed = 0;
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.one * explosionRadius;

        while (timePassed < duration)
        {
            transform.localScale = Vector3.Lerp(startScale, endScale, (timePassed / duration));

            timePassed += Time.deltaTime;
            yield return null;
        }
        transform.localScale = endScale;
        Destroy(gameObject);
        yield return null;
    }



    #region Decrepit
    /*void Explode()
    {
        if(canExplode)
        {
            explosionObject = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            explosionStartTime = Time.time;

            canExplode = false;

            ApplyExplosionForce();
            StartCoroutine(DestroyAfterDuration(duration));
        }
    }

    void ApplyExplosionForce()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider collider in colliders)
        {
            Rigidbody rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 direction = collider.transform.position - transform.position;
                rb.AddForce(transform.TransformDirection(Vector3.up) * explosionUpForce, ForceMode.Impulse);

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }
        }
    }

    void Update()
    {
        if (explosionObject != null && Time.time < explosionStartTime + duration)
        {
            float progress = (Time.time - explosionStartTime) / duration;

            float scale = Mathf.Lerp(0f, explosionRadius * 2, progress);
            explosionObject.transform.localScale = new Vector3(scale, scale, scale);
        }
    }

    IEnumerator DestroyAfterDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        canExplode = true;
        Destroy(explosionObject);
    }

        void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }

*/
    #endregion
}
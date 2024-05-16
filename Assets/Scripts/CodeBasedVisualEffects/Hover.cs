using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] float range = 0.3f;
    [SerializeField] float speed = 5f;
    [SerializeField] bool randomStart = true;
    float randomOffSet = 0;
    Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
        if(randomStart)
        {
            randomOffSet = Random.Range(-range, range);
        }
        
    }

    private void Update()
    {
        float yModifier = range * Mathf.Sin((Time.time + randomOffSet) * speed);
        this.transform.position += new Vector3(0, yModifier, 0) * Time.deltaTime;
    }
}

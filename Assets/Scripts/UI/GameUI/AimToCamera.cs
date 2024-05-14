using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AimToCamera : MonoBehaviour
{
    private void Update()
    {
        Vector3 dir = Camera.main.transform.position - transform.position;
        transform.forward= dir.normalized;
    }
}

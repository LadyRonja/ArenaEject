using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantDelete : MonoBehaviour
{
    private void Awake()
    {
        Destroy(this.gameObject);
    }
}

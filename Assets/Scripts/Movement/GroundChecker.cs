using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundChecker : MonoBehaviour
{
    public bool IsGrounded { get => GetIsGrounded(); }
    private bool GetIsGrounded()
    {
        float groundPlanckDist = 0.05f;
        if (Physics.Raycast(transform.position, Vector3.down, groundPlanckDist))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}

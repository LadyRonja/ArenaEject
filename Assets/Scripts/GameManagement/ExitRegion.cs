using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitRegion : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ReadyBehaivor>(out ReadyBehaivor player))
        {
            player.DisplayExit();
            player.inExitRegion = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<ReadyBehaivor>(out ReadyBehaivor player))
        {
            player.inExitRegion = false;
            player.HideDisplayCanvas();
        }
    }
}

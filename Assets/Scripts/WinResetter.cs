using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinResetter : MonoBehaviour
{
    void Start()
    {
        StaticStats.playerWins = new();
    }
}

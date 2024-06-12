using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Taunt : MonoBehaviour
{
    [SerializeField] AudioClip tauntSound;
    float cooldown = 5f;
    float timer = 0;

    private void OnDPadUp(InputValue value)
    {
        if (timer > 0) { return; }
        AudioHandler.PlaySoundEffect(tauntSound);
        timer = cooldown;
    }

    private void Update()
    {
        if(timer > 0)
        {
            timer-= Time.deltaTime;
        }
    }
}

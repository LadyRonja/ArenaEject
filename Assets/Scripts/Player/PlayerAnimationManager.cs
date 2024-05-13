using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator _anim;
    private Movement _movementController;
    private WeaponUser _weaponUser;
    private GroundChecker _groundCheck;
    private int currentState = 0;

    private int defaultAnimation = 0;
    private static readonly int IDLE = Animator.StringToHash("rig|placeholder_idle");
    private static readonly int IDLE_GUN = Animator.StringToHash("rig|placeholder_idle_weapon_gun");
    private static readonly int RUN = Animator.StringToHash("rig|placeholder_run_nogun");
    private static readonly int RUN_GUN = Animator.StringToHash("rig|placeholder_run_gun");


    private void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        if (_anim == null)
        {
            Debug.LogError("AnimationHandler unable To find player animator component!");
        }

        if (!TryGetComponent<Movement>(out _movementController))
        {
            Debug.LogError("AnimationHandler unable To find player Movement script!");
        }

        if (!TryGetComponent<WeaponUser>(out _weaponUser))
        {
            Debug.LogError("AnimationHandler unable To find player WeaponUser script!");
        }

        if (!TryGetComponent<GroundChecker>(out _groundCheck))
        {
            Debug.LogError("AnimationHandler unable To find player GroundCheck script!");
        }

        defaultAnimation = IDLE;

    }

    private void FixedUpdate()
    {
        if(_anim == null) { return; }
        if(_movementController == null) { return;}
        if(_weaponUser == null) { return; }
        if(_groundCheck == null) { return; }

        // VERY temp way of doing this
        if(_movementController.RB.velocity.sqrMagnitude == 0)
        {
            if(_weaponUser.carriedWeapon == null)
            {
                TrySetAnimation(IDLE);
            }
            else
            {
                TrySetAnimation(IDLE_GUN);
            }
        }
        else
        {
            if (_weaponUser.carriedWeapon == null)
            {

                TrySetAnimation(RUN);
            }
            else
            {
                TrySetAnimation(RUN_GUN);
            }
        }
        
    }

    private void TrySetAnimation(int animation)
    {
        if (currentState != animation)
        {
            _anim.CrossFade(animation, 0f, 0);
            currentState = animation;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerAnimationManager : MonoBehaviour
{
    [HideInInspector] public Animator animator;
    private Movement _movementController;
    private WeaponUser _weaponUser;
    private GroundChecker _groundCheck;
    private KnockBackHandler _knockBackHandler;
    private int currentState = 0;

    private Dictionary<int, int> animationsWithWeaponPairings = new();

    private int defaultAnimation = 0;
    private static string defaultAnimationName = "rig|anim_player_idle";
    private static readonly int IDLE = Animator.StringToHash("rig|anim_player_idle");
    private static readonly int IDLE_GUN = Animator.StringToHash("rig|anim_idle_weapon");
    private static readonly int RUN = Animator.StringToHash("rig|anim_player_run");
    private static readonly int RUN_GUN = Animator.StringToHash("rig|anim_player_run_with_gun");
    private static readonly int JUMP = Animator.StringToHash("rig|anim_jump_start");
    private static readonly int FALL = Animator.StringToHash("rig|anim_jump_midair");

    // Unimplemented
    private static readonly int LAND = Animator.StringToHash("rig|anim_jump_end");
    private static readonly int KNOCKEDBACK = Animator.StringToHash(defaultAnimationName);


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        if (animator == null)
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

        if (!TryGetComponent<KnockBackHandler>(out _knockBackHandler))
        {
            Debug.LogError("AnimationHandler unable To find player KnockBackHandler script!");
        }

        defaultAnimation = IDLE;
        DecalareAnimationPairings();

    }

    private void FixedUpdate()
    {
        ManageAnimations();
    }

    private void DecalareAnimationPairings()
    {
        animationsWithWeaponPairings = new();

        animationsWithWeaponPairings.Add(IDLE, IDLE_GUN);
        animationsWithWeaponPairings.Add(RUN, RUN_GUN);
    }

    private void ManageAnimations()
    {
        if (!ComponentsVerified()) { return; }

        int desiredAnimationState = GetDesiredAnimationState();
        TrySetAnimation(desiredAnimationState);
    }

    private int GetDesiredAnimationState()
    {
        if (!ComponentsVerified()) { return defaultAnimation; }

        int animationToReturn = GetBaseAnimation();

        // Check if this animation has aweapon variant
        if(_weaponUser.carriedWeapon != null)
        {
            foreach (var key in animationsWithWeaponPairings.Keys)
            {
                if (key == animationToReturn)
                {
                    return animationsWithWeaponPairings[key];
                }
            }
        }

        return animationToReturn;

        // Get base animation, ignoring weather or not a weapon is carried
        int GetBaseAnimation()
        {
            if (!ComponentsVerified()) { return defaultAnimation; }

            int animationToReturn = defaultAnimation;

            // Knocked back
            if (_knockBackHandler.KnockedBack)
            {
                return KNOCKEDBACK;
            }

            // Airborn
            if (!_groundCheck.IsGrounded)
            {
                if (_movementController.RB.velocity.y > 0)
                {
                    return JUMP;
                }
                else
                {
                    return FALL;
                }
            }

            // Movement
            if (_movementController.RB.velocity.sqrMagnitude <= 0.1f)
            {
                return IDLE;
            }
            else
            {
                return RUN;
            }
        }
    }

    private bool ComponentsVerified()
    {
        if (animator == null) { return false; }
        if (_movementController == null) { return false; }
        if (_weaponUser == null) { return false; }
        if (_groundCheck == null) { return false; }
        if (_knockBackHandler == null) { return false; }

        return true;
    }

    private void TrySetAnimation(int animation)
    {
        if (currentState != animation)
        {
            animator.CrossFade(animation, 0f, 0);
            currentState = animation;
        }
    }
}

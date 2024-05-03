using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.Rendering.DebugUI;

public class WeaponUser : MonoBehaviour 
{
    [HideInInspector] public bool appropriatlySpawned = false;

    [SerializeField] private Transform weaponCarryPoint;
    [SerializeField] private float weaponLaunchForce = 8f;
    public Transform carriedWeaponTransform;
    private GameObject weaponUser;

    public Weapon carriedWeapon = null;
    [HideInInspector] public int shotsFired = 0;

    private bool shooting = false;

    [SerializeField] private float angleToThrowExplosiveWeapon = 45f;
    [SerializeField] private float weaponLaunchForceExplosive = 40f;
    private float weaponThrowForwardOffset = 1.5f;
    private GroundChecker groundChecker;
    [SerializeField] private GameObject weaponThrowAssisst;

    private void Start()
    {
        groundChecker = GetComponent<GroundChecker>();
        HideThrowAimAssist();
    }

    private void Update()
    {
        if(shooting)
        {
            TryFireWeapon();
        }

        if(!groundChecker.IsGrounded)
        {
            DisplayThrowAimAssist();
        }
        else
        {
           HideThrowAimAssist();
        }
    }

    private void OnFire(InputValue value)
    {
        if(value.Get<float>() > 0.5f) {
            shooting = true;
            //TryFireWeapon();
        }
        else {
            shooting= false;
        }
    }
    private void OnNorthButtonDown(InputValue value)
    {
        if(groundChecker == null)
        {
            ThrowWeapon();
            return;
        }

        if(groundChecker.IsGrounded) {
            ThrowWeapon();
        }
        else {
            ThrowWeapon(true);
        }
    }

    private void TryFireWeapon()
    {
        if (carriedWeapon == null) return;

        bool fireSuccess = carriedWeapon.TryShoot();

        if (fireSuccess)
            shotsFired++;
    }

    public bool AttemptAquireWeapon(Weapon weaponPrefabToAquire)
    {
        if(carriedWeapon == null)
        {
            AquireWeapon(weaponPrefabToAquire);
            return true;
        }

        if (carriedWeapon.ammoCount <= 0)
        {
            AquireWeapon(weaponPrefabToAquire);
            return true;
        }

        return false;
    }

    private void AquireWeapon(Weapon weaponPrefabToAquire)
    {
        if(carriedWeapon!= null)
        {
            Destroy(carriedWeapon.gameObject);
            carriedWeapon = null;
        }

        GameObject weaponObj = Instantiate(weaponPrefabToAquire).gameObject;
        Weapon weaponScr = weaponObj.GetComponent<Weapon>();
        Transform weaponCarryParent;
        if (weaponCarryPoint != null)   { weaponCarryParent = weaponCarryPoint; }
        else                            { weaponCarryParent = transform; }
        weaponObj.transform.SetParent(weaponCarryParent);
        weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        carriedWeapon = weaponScr;
    }

    private void ThrowWeapon(bool throwWeaponToExplode = false)
    {
        if (carriedWeapon == null) return;
        if(throwWeaponToExplode)
        {
            //if (carriedWeapon.ammoCount > 0) { return; } // TODO: Once ammo count is balanced, enable this check.
        }

        Collider collider = carriedWeapon.GetComponent<Collider>();
        if (collider == null)
        {
            Destroy(carriedWeapon.gameObject);
            carriedWeapon = null;
            return;
        }

        Vector3 dropPosition = transform.position + transform.forward * weaponThrowForwardOffset;
        dropPosition.y += 1f;
        Quaternion dropRotation = Quaternion.Euler(0f, 90f, 0f);
        if (throwWeaponToExplode)
        {
            dropRotation = Quaternion.Euler(angleToThrowExplosiveWeapon, 0, 0f);
        }

        carriedWeapon.gameObject.SetActive(true);
        carriedWeapon.transform.position = dropPosition;
        carriedWeapon.transform.rotation *= dropRotation;
        carriedWeapon.transform.SetParent(null);
        collider.enabled = true;

        Rigidbody wpRb = carriedWeapon.AddComponent<Rigidbody>();

        if (wpRb != null)
        {
            Vector3 launchDirection = gameObject.transform.forward;
            if (throwWeaponToExplode) {
                launchDirection = carriedWeapon.transform.forward.normalized;
            }

            if (throwWeaponToExplode)
            {
                wpRb.useGravity = false;
                wpRb.AddForce(launchDirection * weaponLaunchForceExplosive, ForceMode.Impulse);
                if(TryGetComponent<Rigidbody>(out Rigidbody userRb))
                {
                    wpRb.AddForce(userRb.velocity, ForceMode.Impulse);
                }
                carriedWeapon.ReadyExplodeOnImpact();
            }
            else {
                wpRb.AddForce(launchDirection * weaponLaunchForce, ForceMode.Impulse);
                wpRb.AddForce(transform.TransformDirection(Vector3.up) * 3f, ForceMode.Impulse);
                if (TryGetComponent<Rigidbody>(out Rigidbody userRb))
                {
                    wpRb.AddForce(userRb.velocity, ForceMode.Impulse);
                }
            }
            collider.isTrigger = false;
        }

        carriedWeapon = null;
    }

    private void DisplayThrowAimAssist()
    {
        if (weaponThrowAssisst == null) { return; }
        if (carriedWeapon == null){ return; }

        Vector3 throwPosition = transform.position + transform.forward * weaponThrowForwardOffset;
        throwPosition.y += 1f;

        Vector3 throwRotation = transform.forward;
        Vector3 throwangle = new Vector3(0f, angleToThrowExplosiveWeapon, 0f).normalized;

        throwRotation.y -= throwangle.y;

        if (Physics.Raycast(throwPosition, throwRotation, out RaycastHit hit))
        {
            weaponThrowAssisst.SetActive(true);
            Vector3 hitPosition = hit.point;
            hitPosition.y += 0.5f;
            weaponThrowAssisst.transform.position = hit.point;
        }
    }

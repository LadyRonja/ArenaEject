using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponUser : MonoBehaviour 
{
    [HideInInspector] public bool appropriatlySpawned = false;

    [SerializeField] private Transform weaponCarryPoint;
    [SerializeField] private float weaponLaunchForce = 8f;
    public Transform carriedWeaponTransform;

    public Weapon carriedWeapon = null;
    [HideInInspector] public int shotsFired = 0;

    bool shooting = false;

    private void Update()
    {
        if(shooting)
        {
            TryFireWeapon();
        }
    }

    private void OnFire(InputValue value)
    {
        if(value.Get<float>() > 0.5f)
        {
            shooting = true;
            //TryFireWeapon();
        }
        else
        {
            shooting= false;
        }
    }
    private void OnNorthButtonDown(InputValue value)
    {
        ThrowWeapon();
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

    private void ThrowWeapon()
    {
        if (carriedWeapon == null) return;

        Collider collider = carriedWeapon.GetComponent<Collider>();   
        if(collider == null)
        {
            Destroy(carriedWeapon.gameObject);
            carriedWeapon = null;
            return;
        }

        Vector3 dropPosition = transform.position + transform.forward * 1.5f;
        dropPosition.y += 1f;
        Quaternion dropRotation = Quaternion.Euler(0f, 90f, 0f);

        carriedWeapon.gameObject.SetActive(true);
        carriedWeapon.transform.position = dropPosition;
        carriedWeapon.transform.rotation *= dropRotation;
        carriedWeapon.transform.SetParent(null);
        collider.enabled = true;

        Rigidbody wpRb = carriedWeapon.AddComponent<Rigidbody>();

        if (wpRb != null)
        {
            Vector3 launchDirection = gameObject.transform.forward;
            wpRb.AddForce(launchDirection * weaponLaunchForce, ForceMode.Impulse);
            wpRb.AddForce(transform.TransformDirection(Vector3.up) * 3f, ForceMode.Impulse);
            collider.isTrigger = false;
        }

        carriedWeapon = null;
    }

    #region Decrepit
    /*
     
    public List<Weapon> inventory = new List<Weapon>();
    private bool isNearWeapon = false;
    private Weapon nearbyWeapon;
    private Vector3 carriedWeaponOffset;


    void Update()
    {

        if(inventory.Count > 0)
         {
             UpdateCarriedWeaponPosition();
         }
    }
    
    void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Weapon"))
    {
        isNearWeapon = true;
        nearbyWeapon = other.GetComponent<Weapon>();
        PickupWeapon();
    }
}

void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Weapon"))
    {
        isNearWeapon = false;
        nearbyWeapon = null;
    }


    public void PickupWeapon()
    {
        if (nearbyWeapon != null)
        {
            inventory.Add(nearbyWeapon);
            nearbyWeapon.transform.SetParent(transform);
            nearbyWeapon.transform.localPosition = Vector3.zero;
            nearbyWeapon.transform.rotation = Quaternion.Euler(0f, 90f, 0f);
        }
    }

     private void UpdateCarriedWeaponPosition()
     {
         foreach (Weapon weapon in inventory)
         {
             if(weapon != null)
             {
                 carriedWeaponOffset = new Vector3(0.6f, 1f, 1);
                 weapon.transform.position = carriedWeaponTransform.position + carriedWeaponOffset;
             }
         }
     }*/
    #endregion
}
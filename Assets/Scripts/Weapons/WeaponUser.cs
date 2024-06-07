using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class WeaponUser : MonoBehaviour
{
    [HideInInspector] public bool appropriatlySpawned = false;

    [SerializeField] public Transform weaponCarryPoint;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private float weaponLaunchForce = 8f;
    public Transform carriedWeaponTransform;

    public Weapon carriedWeapon = null;
    [HideInInspector] public int shotsFired = 0;

    private bool shooting = false;
    [HideInInspector] public int userIndex = -1;

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
        if (shooting)
        {
            TryFireWeapon();
        }

        if (!groundChecker.IsGrounded)
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
        if (value.Get<float>() > 0.5f)
        {
            shooting = true;
            //TryFireWeapon();
        }
        else
        {
            shooting = false;
        }
    }
    private void OnRightBumperDown(InputValue value)
    {
        OnNorthButtonDown(value);
    }

    private void OnNorthButtonDown(InputValue value)
    {
        if (groundChecker == null)
        {
            ThrowWeapon();
            return;
        }

        if (groundChecker.IsGrounded)
        {
            ThrowWeapon();
        }
        else
        {
            ThrowWeapon(true);
        }
    }

    private void TryFireWeapon()
    {
        if (carriedWeapon == null) return;

        bool fireSuccess = carriedWeapon.TryShoot();

        if (fireSuccess)
        {
            shotsFired++;
            PlayerShooting.shotsFiredPerPlayer[userIndex] = shotsFired;
        }
    }

    public bool AttemptAquireWeapon(Weapon weaponPrefabToAquire)
    {
        if (carriedWeapon == null)
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
        if (carriedWeapon != null)
        {
            Destroy(carriedWeapon.gameObject);
            carriedWeapon = null;
        }

        GameObject weaponObj = Instantiate(weaponPrefabToAquire).gameObject;
        Weapon weaponScr = weaponObj.GetComponent<Weapon>();
        weaponScr.ownerIndex = userIndex;
        Transform weaponCarryParent;
        if (weaponCarryPoint != null) { weaponCarryParent = weaponCarryPoint; }
        else { weaponCarryParent = transform; }
        weaponObj.transform.SetParent(weaponCarryParent);
        weaponObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        weaponObj.transform.localRotation = Quaternion.Euler(-90f, 0, 180f);

        carriedWeapon = weaponScr;
    }

    private void ThrowWeapon(bool throwWeaponToExplode = false)
    {
        if (throwWeaponToExplode)
        {
            //if (carriedWeapon.ammoCount > 0) { return; } // TODO: Once ammo count is balanced, enable this check.
        }
        carriedWeapon.ownerIndex = -1;
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

            Vector3 explosiveRotation = transform.forward;
            if (throwWeaponToExplode)
            {
                dropRotation = Quaternion.Euler(angleToThrowExplosiveWeapon, 0, 0f);

                Vector3 angle = Vector3.Cross(explosiveRotation, Vector3.down);
                explosiveRotation = Quaternion.AngleAxis(angleToThrowExplosiveWeapon, angle) * explosiveRotation;
            }

            if (throwWeaponToExplode)
            {
                wpRb.useGravity = false;
                wpRb.AddForce(explosiveRotation.normalized * weaponLaunchForceExplosive, ForceMode.Impulse);
                if (TryGetComponent<Rigidbody>(out Rigidbody userRb))
                {
                    wpRb.AddForce(userRb.velocity, ForceMode.Impulse);
                }
                carriedWeapon.ReadyExplodeOnImpact();
            }
            else
            {
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
        if (weaponThrowAssisst == null) { HideThrowAimAssist(); return; }
        if (carriedWeapon == null) { HideThrowAimAssist(); return; }

        Vector3 throwPosition = transform.position + transform.forward * weaponThrowForwardOffset;
        throwPosition.y += 1f;

        Vector3 throwRotation = transform.forward;
        Vector3 angle = Vector3.Cross(throwRotation, Vector3.down);
        throwRotation = Quaternion.AngleAxis(angleToThrowExplosiveWeapon, angle) * throwRotation;

        if (Physics.Raycast(throwPosition, throwRotation, out RaycastHit hit))
        {
            weaponThrowAssisst.SetActive(true);
            Vector3 hitPosition = hit.point;
            hitPosition.y += 1.5f;
            weaponThrowAssisst.transform.position = hitPosition;
        }
    }

    private void HideThrowAimAssist()
    {
        if (weaponThrowAssisst == null) { return; }
        if (!weaponThrowAssisst.gameObject.activeSelf) { return; }
        weaponThrowAssisst.SetActive(false);
    }

    public void BotFire(bool fire)
    {
        shooting = fire;
    }

    public void BotThrow()
    {
        if (groundChecker == null)
        {
            ThrowWeapon();
            return;
        }

        if (groundChecker.IsGrounded)
        {
            ThrowWeapon();
        }
        else
        {
            ThrowWeapon(true);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.SceneManagement;

public class Bot : MonoBehaviour
{
    private Movement movementController;
    private Aiming aimController;
    private WeaponUser weaponUser;
    private Jump jumpController;
    private PlayerStats myStats;
    private bool setUpComplete = false;

    private List<PickUpSpawner> spawnerList = new();
    private List<PlayerStats> opponents = new();

    Vector3 movingToPosition = Vector3.zero;
    Vector3 startPos = Vector3.zero;
    PlayerStats targetedPlayer;

    private void Start()
    {
        if(SceneManager.GetActiveScene().name == Paths.START_SCENE_NAME)
        {
            this.enabled = false;
            return;
        }

        startPos = transform.position;
        Invoke(nameof(SetUp), 3f);
    }

    private void Update()
    {
        if (!setUpComplete)
        {
            return;
        }

        Vector3 moveDir =  movingToPosition - transform.position;
        moveDir.Normalize();
        movementController.BotMovement(new Vector2(moveDir.x, moveDir.z));

        if ((transform.position - movingToPosition).sqrMagnitude < 0.2f)
        {
            PickRandomPosition();
        }

        if(targetedPlayer != null && weaponUser.carriedWeapon != null)
        {
            Vector3 aimDir = targetedPlayer.transform.position - transform.position;
            aimDir.Normalize();
            aimController.BotAiming(new Vector2(aimDir.x, aimDir.z));
        }
        else if (targetedPlayer != null)
        {
            PickRandomPlayerTarget();
        }

        if(weaponUser.carriedWeapon != null)
        {

            weaponUser.BotFire(weaponUser.carriedWeapon.ammoCount != 0);
        }

        if(Input.GetKeyDown(KeyCode.M) && Input.GetKey(KeyCode.P))
        {
            Debug.Log("Trying to clone player");
            Instantiate(this.gameObject, startPos, Quaternion.identity);

            SetUp();

            PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
            List<Transform> transforms = new();
            foreach (PlayerStats p in allPlayers)
            {
               transforms.Add(p.transform);
            }
            CameraController.Instance.StartTrackingObjects(transforms);
        }
    }

    private void OnDisable()
    {
        this.CancelInvoke();
    }

    private void OnDestroy()
    {
        this.CancelInvoke();
    }

    private void SetUp()
    {
        if (!TryGetComponent<PlayerStats>(out myStats))
        {
            this.enabled = false;
            return;
        }
        else
        {
            if(myStats.playerIndex != 2)
            {
                this.enabled = false;
                return;
            }
        }

        if (!TryGetComponent<Movement>(out movementController))
        {
            this.enabled = false;
            return;
        }

        if (!TryGetComponent<Aiming>(out aimController))
        {
            this.enabled = false;
            return;
        }

        if (!TryGetComponent<WeaponUser>(out weaponUser))
        {
            this.enabled = false;
            return;
        }

        if (!TryGetComponent<Jump>(out jumpController))
        {
            this.enabled = false;
            return;
        }
        JumpRandomly();

        // Piuckup locations
        PickUpSpawner[] pickUpSpawners = (PickUpSpawner[])FindObjectsOfType(typeof(PickUpSpawner));
        foreach (PickUpSpawner pus in pickUpSpawners)
        {
            if(Vector3.Distance(pus.transform.position, transform.position) < 15f)
            {
                spawnerList.Add(pus);
            }
        }
        PickRandomPosition();


        // Opponents
        PlayerStats[] allPlayers = (PlayerStats[])FindObjectsOfType(typeof(PlayerStats));
        foreach (PlayerStats p in allPlayers)
        {
            if(p != myStats)
            {
                opponents.Add(p);
            }
        }
        PickRandomPlayerTarget();




        setUpComplete = true;
    }

    private void PickRandomPosition()
    {
        this.CancelInvoke(nameof(PickRandomPosition));
        if(weaponUser.carriedWeapon == null || weaponUser.carriedWeapon.ammoCount < 2)
        {
            int randPos = UnityEngine.Random.Range(0, spawnerList.Count);
            movingToPosition = spawnerList[randPos].transform.position;
        }
        else if (targetedPlayer != null)
        {
            movingToPosition = targetedPlayer.transform.position;
        }
        else
        {
            movingToPosition = startPos;
        }

        float randNewPosInTime = UnityEngine.Random.Range(5f, 10f);
        Invoke(nameof(PickRandomPosition), randNewPosInTime);
    }

    private void PickRandomPlayerTarget()
    {
        int safetyIndex = 0;

        do
        {
            int randPlayer = UnityEngine.Random.Range(0, opponents.Count);
            targetedPlayer = opponents[randPlayer];

            safetyIndex++;
            if(safetyIndex > 25)
            {
                break;
            }
        }
        while (!targetedPlayer.gameObject.activeSelf);

        float newPlayerInTime = UnityEngine.Random.Range(3f, 13f);
        Invoke(nameof(PickRandomPlayerTarget), newPlayerInTime);
    }

    private void JumpRandomly()
    {
        this.CancelInvoke(nameof(JumpRandomly));
        jumpController.BotJump();

        if((int)UnityEngine.Random.Range(0, 100) > 20)
        {
            Invoke(nameof(ThrowWeapon), 0.5f);
        }

        float jumpTimer = UnityEngine.Random.Range(5f, 20f);
        Invoke(nameof(JumpRandomly), jumpTimer);
    }

    private void ThrowWeapon()
    {
        if(weaponUser.carriedWeapon!= null)
        {
            weaponUser.BotThrow();
        }
    }
}

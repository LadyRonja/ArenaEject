using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Ammo : MonoBehaviour
{
    [SerializeField] protected float destroyDelay = 10f;

    public float moveSpeed = 10f;
    [HideInInspector] public Vector3 moveDir = Vector3.forward;
    [SerializeField] protected float knockbackForce = 15f;

    [Header("Aim Assist")]
    [SerializeField] protected bool useAimAssist = true;
    protected float aimAssistLength = 10f;
    protected float aimAssistSpeed = 0.7f;
    
    protected Rigidbody rb;

    protected virtual void Start()
    {
        DestroyAfterDelay();
        rb = GetComponent<Rigidbody>();
        rb.velocity = moveDir * moveSpeed;
    }

    protected void Update()
    {
        AimAssist();
    }

    protected void AimAssist()
    {
        if(!useAimAssist) { return; }

        List<Vector3> playerPosInSight = new();
        for (int i = -10; i < 10; i++)
        {
            Vector3 rayStartPos = transform.position + (transform.right * i* 0.2f);

            Debug.DrawRay(rayStartPos, transform.forward * aimAssistLength, Color.magenta, Time.deltaTime);

            if(Physics.Raycast(rayStartPos, transform.forward, out RaycastHit hit, aimAssistLength))
            {
                if(hit.collider.gameObject.TryGetComponent<PlayerStats>(out PlayerStats playerInVision))
                {
                    playerPosInSight.Add(playerInVision.transform.position);
                }
            }
        }


        if (playerPosInSight.Count > 0)
        {
            Vector3 closestPlayer = playerPosInSight[0];
            float distanceToClosest = (closestPlayer - transform.position).sqrMagnitude;

            foreach (Vector3 playerPos in playerPosInSight)
            {
                float distanceTo = (playerPos - transform.position).sqrMagnitude;
                if (distanceTo < distanceToClosest)
                {
                    distanceToClosest= distanceTo;
                    closestPlayer = playerPos;
                }
            }

            Vector3 targetDir = closestPlayer - transform.position;
            targetDir.Normalize();
            Vector3 newAim = Vector3.Lerp(rb.velocity.normalized, targetDir, aimAssistSpeed);
            newAim.y = 0;
            transform.forward = newAim;
            rb.velocity = newAim * moveSpeed;
        }
    }

    protected virtual void DestroyAfterDelay()
    {
        Destroy(gameObject, destroyDelay);
    }
}

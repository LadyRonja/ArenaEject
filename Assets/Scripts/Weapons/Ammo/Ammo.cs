using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public abstract class Ammo : MonoBehaviour
{
    [SerializeField] protected float destroyDelay = 10f;
    public float moveSpeed = 10f;


    [Header("Knockback")]
    [SerializeField] protected float knockbackForce = 15f;
    [SerializeField] protected bool knockbackLocksPlayer = true;


    [Header("Aim Assist")]
    [SerializeField] protected bool useAimAssist = true;
    [SerializeField] protected float aimAssisstLookAhead = 10f;
    [SerializeField] protected float aimAssisstWidth = 10f;
    [SerializeField] protected float aimAssistSpeed = 0.7f;
    
    protected Rigidbody rb;
    [HideInInspector] public Vector3 moveDir = Vector3.forward;
    [HideInInspector] public int ownerIndex = -1;

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
        for (float i = (-1f * Mathf.Abs(aimAssisstWidth)); i < Mathf.Abs(aimAssisstWidth); i++)
        {
            Vector3 rayStartPos = transform.position + (transform.right * (i * 0.2f));

            Debug.DrawRay(rayStartPos, transform.forward * aimAssisstLookAhead, Color.magenta, Time.deltaTime);

            if(Physics.Raycast(rayStartPos, transform.forward, out RaycastHit hit, aimAssisstLookAhead))
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
            Vector3 newAim = Vector3.Lerp(rb.velocity.normalized, targetDir, aimAssistSpeed * Time.deltaTime);
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

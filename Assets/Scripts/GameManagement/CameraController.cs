using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CameraModes { STILL, TRACK_OBJECTS, SHAKE}

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance { get => instance; }

    [Header("Generic")]
    [SerializeField] CameraModes cameraMode = CameraModes.STILL;
    [SerializeField] float xOffSet = 0f;
    [SerializeField] float yOffSet = 0f;
    [SerializeField] float zOffSet = 0f;
    [Space]
    [SerializeField] float xAngleDeg = 35f;


    [Header("Track Objects")]
    [SerializeField] float screenPadding = 1f;
    [SerializeField] List<Transform> transformsToTrack;

    Vector3 targetPos = Vector3.zero;

    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
            targetPos = transform.position;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public float DEBUG_ShakeAmount = 5f;
    public float DEBUG_ShakeDuration = 0.5f;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddShake(DEBUG_ShakeAmount, DEBUG_ShakeDuration);
        }
    }

    private void FixedUpdate()
    {
        switch (cameraMode)
        {
            case CameraModes.STILL:
                break;
            case CameraModes.TRACK_OBJECTS:
                TrackObjects();
                break;
            case CameraModes.SHAKE:
                break;
            default:
                break;
        }
    }

    public void AddShake(float amount, float duration)
    {
        StartCoroutine(ShakeRoutine(amount, duration));
    }

    private IEnumerator ShakeRoutine(float amount, float duration)
    {
        float timePassed = 0f;

        while (timePassed < duration)
        {
            transform.position = targetPos + Random.insideUnitSphere * amount;

            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public void StartTrackingObjects(List<Transform> objectsToTrack)
    {
        transformsToTrack = objectsToTrack;
        cameraMode = CameraModes.TRACK_OBJECTS;
    }

    private void TrackObjects()
    {
        if (transformsToTrack.Count < 1) { return; }

        Vector3 targetMaxs = transformsToTrack[0].position;
        Vector3 targetMins = transformsToTrack[0].position;
        for (int i = 0; i < transformsToTrack.Count; i++)
        {
            targetMaxs.x = Mathf.Max(targetMaxs.x, transformsToTrack[i].position.x);
            targetMaxs.y = Mathf.Max(targetMaxs.y, transformsToTrack[i].position.y);
            targetMaxs.z = Mathf.Max(targetMaxs.z, transformsToTrack[i].position.z);

            targetMins.x = Mathf.Min(targetMins.x, transformsToTrack[i].position.x);
            targetMins.y = Mathf.Min(targetMins.y, transformsToTrack[i].position.y);
            targetMins.z = Mathf.Min(targetMins.z, transformsToTrack[i].position.z);
        }

        Vector3 centerPoint = (targetMaxs + targetMins) * 0.5f;

        targetPos = centerPoint;
        targetPos.y += 15f;
        targetPos.z -= 15f;
        transform.transform.position = targetPos;
    }
}

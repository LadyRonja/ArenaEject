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

    //Vector3 targetPos = Vector3.zero;

    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        Invoke(nameof(_temp), 0.5f);
    }

    private void _temp()
    {
        var players = FindObjectsOfType<PlayerStats>().ToList();
        List<Transform> playerTransforms = new List<Transform>();
        foreach (var t in players)
        {
            playerTransforms.Add(t.transform);
        }
        StartTrackingObjects(playerTransforms);
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

        Vector3 tartgetPos = centerPoint;
        tartgetPos.y += 15f;
        tartgetPos.z -= 15f;
        transform.transform.position = tartgetPos;
    }
}

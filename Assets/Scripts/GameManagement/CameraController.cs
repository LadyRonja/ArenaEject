using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CameraModes { STILL, TRACK_OBJECTS, SHAKE}

public struct TrackPoint{
    public TrackPoint(Vector3 position, DateTime savedAt, float lifeTime)
    {
        this.position = position;
        this.savedAt= savedAt;
        this.lifeTime = lifeTime;
    }

    public Vector3 position;
    public DateTime savedAt;
    private float lifeTime;
    public bool Expired { get => DateTime.Now > savedAt.AddSeconds(lifeTime); }
}

public class CameraController : MonoBehaviour
{
    private static CameraController instance;
    public static CameraController Instance { get => instance; }

    [Header("Generic")]
    [SerializeField] CameraModes cameraMode = CameraModes.STILL;
    //[SerializeField] float xOffSet = 0f;
    //[SerializeField] float yOffSet = 0f;
    //[SerializeField] float zOffSet = 0f;
    [Space]
    [SerializeField] float xAngleDeg = 35f;
    [Space]
    [SerializeField] bool displayDebugs = false;

    [Header("Track Objects")]
    [Range(0.01f, 0.99f)][SerializeField] float trackSpeedMinimum = 0.1f;
    //[SerializeField] float maxZoom = 3f; // TODO
    [SerializeField] float paddingWidth = 2f;
    [SerializeField] float paddingTop = 1f;
    [SerializeField] float paddingBottom = 8f;
    [SerializeField] List<Transform> transformsToTrack;
    private List<TrackPoint> delayedExtremes= new List<TrackPoint>();
    [SerializeField] float lingerTime = 0.5f;
    float camTrackSpeed = 0.1f; // dynamically changes, saved value only sets initial

    [Header("Shake Debug")]
    public float DEBUG_ShakeAmount = 5f;
    public float DEBUG_ShakeDuration = 0.5f;

    Vector3 targetPos = Vector3.zero;

    private void Awake()
    {
        if(instance == null || instance == this)
        {
            instance = this;
            targetPos = transform.position;
            transform.rotation = Quaternion.Euler(xAngleDeg, 0f, 0f);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            AddShake(DEBUG_ShakeAmount, DEBUG_ShakeDuration);
        }
    }

    private void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(xAngleDeg, 0f, 0f);
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
            transform.position = targetPos + UnityEngine.Random.insideUnitSphere * amount;

            timePassed += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public void StartTrackingObjects(List<Transform> objectsToTrack)
    {
        transformsToTrack = objectsToTrack;
        delayedExtremes.Clear();
        cameraMode = CameraModes.TRACK_OBJECTS;
    }

    private void TrackObjects()
    {
        if (transformsToTrack.Count < 1) { return; }

        // Find the largest and smallest exremes of each tracked object
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

        // Save max positions for a linger time for a less frantic camera
        delayedExtremes.Add(new TrackPoint(targetMaxs, DateTime.Now, lingerTime));
        delayedExtremes.Add(new TrackPoint(targetMins, DateTime.Now, lingerTime));       
        delayedExtremes.RemoveAll(de => de.Expired);
        foreach (TrackPoint tp in delayedExtremes)
        {
            targetMaxs.x = Mathf.Max(targetMaxs.x, tp.position.x);
            targetMaxs.y = Mathf.Max(targetMaxs.y, tp.position.y);
            targetMaxs.z = Mathf.Max(targetMaxs.z, tp.position.z);

            targetMins.x = Mathf.Min(targetMins.x, tp.position.x);
            targetMins.y = Mathf.Min(targetMins.y, tp.position.y);
            targetMins.z = Mathf.Min(targetMins.z, tp.position.z);
        }

        // Find middle point between players
        Vector3 centerPoint = (targetMaxs + targetMins) * 0.5f;
        targetPos = centerPoint;

        // Padding
        float paddedRightMost = targetMaxs.x + paddingWidth;
        float paddedLeftMost = targetMins.x - paddingWidth;
        float paddedTopMost = targetMaxs.z + paddingTop;
        float paddedBottomMost = targetMins.z - paddingBottom;

        // Frustrums
        float frustrumDesiredWidth = paddedRightMost - paddedLeftMost;
        float frustrumDesiredHeight = paddedTopMost - paddedBottomMost;

        // Determine the zoom distance needed to encompass the largest frustrum
        float distanceHypotonuseHeight = (frustrumDesiredHeight * 0.5f) / (Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad));
        float distanceHypotonuseWidth = (frustrumDesiredWidth * 0.5f) / (Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad));
        float distanceHypotonuse = Mathf.Max(distanceHypotonuseHeight, distanceHypotonuseWidth);

        targetPos -= transform.forward * distanceHypotonuse;

        #region Debug Drawing
        if (displayDebugs) {
            float squareY = centerPoint.y;

            Vector3 squareOriginalCornerNW = new Vector3(targetMins.x, squareY, targetMaxs.z);
            Vector3 squareOriginalCornerNE = new Vector3(targetMaxs.x, squareY, targetMaxs.z);
            Vector3 squareOriginalCornerSE = new Vector3(targetMaxs.x, squareY, targetMins.z);
            Vector3 squareOriginalCornerSW = new Vector3(targetMins.x, squareY, targetMins.z);
            DrawDebugSquare(squareOriginalCornerNW, squareOriginalCornerNE, squareOriginalCornerSE, squareOriginalCornerSW, Color.magenta);
        }
        #endregion

        // If any of the extremes are outside of the cameras view,
        // increase the camera interporlationspeed
        // Otherwise decrease it
        // Then clamp it
        float camAcceleration = 0.05f; // Magic number, fast enough to catch most items, slow enough to not be sowewhat smooth
        if (!_IsInViewRange(targetMins) || !_IsInViewRange(targetMaxs)) {
            camTrackSpeed += camAcceleration;
        }
        else {
            camTrackSpeed -= camAcceleration;
        }

        // Clamp track speed interporlation (values outside of 0-1 does nothing)
        camTrackSpeed = Mathf.Clamp(camTrackSpeed, trackSpeedMinimum, 1f);

        // Move camera
        transform.position = Vector3.Lerp(transform.position, targetPos, camTrackSpeed);

        // Determine if a point in the world is within the screens viewport.
        bool _IsInViewRange(Vector3 point)
        {
            Vector3 viewportPoint = Camera.main.WorldToViewportPoint(point);

            if (viewportPoint.x > 1 || viewportPoint.x < 0)
            {
                return false;
            }
            else if (viewportPoint.y > 1 || viewportPoint.y < 0)
            {
                return false;
            }
            else if(viewportPoint.z < 0)
            {
                return false;
            }

            return true;
        }
    }

    private void DrawDebugSquare(Vector3 squareCornerNW, Vector3 squareCornerNE, Vector3 squareCornerSE, Vector3 squareCornerSW, Color color)
    {
        float squareDrawTime = Time.deltaTime;
        Debug.DrawLine(squareCornerNW, squareCornerNE, color, squareDrawTime);
        Debug.DrawLine(squareCornerNE, squareCornerSE, color, squareDrawTime);
        Debug.DrawLine(squareCornerSE, squareCornerSW, color, squareDrawTime);
        Debug.DrawLine(squareCornerSW, squareCornerNW, color, squareDrawTime);
    }
}

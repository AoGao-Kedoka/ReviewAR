using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Google.XR.ARCoreExtensions;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using TMPro;
using MarkupAttributes;
using UnityEditor;

#if UNITY_EDITOR
using MarkupAttributes.Editor;
#endif

/// <summary>
/// Controller for Geospatial sample.
/// </summary>
public class ARController : MonoBehaviour
{
#if UNITY_EDITOR
    [CustomEditor(typeof(ARController)), CanEditMultipleObjects]
    internal class ARControllerEditor : MarkedUpEditor {}
#endif

    [Box("AR")]

    [TitleGroup("AR/AR Properties")]
    /// <summary>
    /// The ARCoreExtensions used .
    /// </summary>
    [SerializeField] ARCoreExtensions _arCoreExtension;

    /// <summary>
    /// AR anchor
    /// </summary>
    [SerializeField] ARAnchorManager _arAnchorManager;

    [TitleGroup("AR/Geospatial Properties")]
    /// <summary>
    /// The AREarthManager used .
    /// </summary>
    [SerializeField] AREarthManager _arEarthManager;

    [EndGroup("AR")]


    [Header("UI")]
    [SerializeField] private UIController _uiController;


    [Box("Debugger")]
    [SerializeField] private DebugController _debugController;
    [SerializeField] private bool _lockScreenToPortrait = true;
    [SerializeField] private float _searchRadius;
    [SerializeField] private GameObject _debuggerPrefab;
    [SerializeField] private GameObject _arrow;
    [EndGroup("Debugger")]

#region Google Maps
    // <summary>
    /// indicate when async task has returned results, set it back to false when finished
    /// </summary>
    private bool _task_finished = false;

    Task<PlacesApiQueryResponse> placesTask = null;
    /// <summary>
    /// places got from the api requests
    /// </summary>
    private PlacesApiQueryResponse _places = null;
#endregion

    private bool _isReturning = false;
    private bool _enablingGeospatial = false;
    private float _configurePrepareTime = 3f;

    bool _isReady = false;
    public bool IsReady { get { return _isReady; } }


    private IEnumerator _startLocationService = null;

    /// <summary>
    /// Accuracy threshold for orientation yaw accuracy in degrees that can be treated as
    /// localization completed.
    /// </summary>
    private const double _orientationYawAccuracyThreshold = 25;

    /// <summary>
    /// Accuracy threshold for heading degree that can be treated as localization completed.
    /// </summary>
    private const double _headingAccuracyThreshold = 25;

    /// <summary>
    /// Accuracy threshold for altitude and longitude that can be treated as localization
    /// completed.
    /// </summary>
    private const double _horizontalAccuracyThreshold = 20;

    /// <summary>
    /// last saved pose
    /// </summary>
    private Vector2 _lastSavedPosition = Vector2.zero;

    /// <summary>
    /// request counter
    /// </summary>
    private int _requestCounter = 0;


    /// <summary>
    /// history of instantiated anchor's name
    /// </summary>
    private List<string> _anchorsHistory = new List<string>();

    public void Awake()
    {
        if (_lockScreenToPortrait)
        {
            // Lock screen to portrait.
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.orientation = ScreenOrientation.Portrait;
        }

        Application.targetFrameRate = 60;

        if (_arCoreExtension == null)
        {
            Debug.LogError("Cannot find ARCoreExtensions.");
        }

    }

    /// <summary>
    /// Unity's OnEnable() method.
    /// </summary>
    public void OnEnable()
    {
        _isReturning = false;
        _enablingGeospatial = false;
        _isReady = false;
        _startLocationService = StartLocationService();
        StartCoroutine(_startLocationService);
    }
    private bool _waitingForLocationService = false;
    private IEnumerator StartLocationService()
    {
        _waitingForLocationService = true;
#if UNITY_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Debug.Log("Requesting fine location permission.");
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3.0f);
        }
#endif

        if (!Input.location.isEnabledByUser)
        {
            Debug.Log("Location service is disabled by User.");
            _waitingForLocationService = false;
            yield break;
        }

        Debug.Log("Start location service.");
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        _waitingForLocationService = false;
        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarningFormat(
                "Location service ends with {0} status.", Input.location.status);
            Input.location.Stop();
        }
    }
    /// <summary>
    /// Unity's OnDisable() method.
    /// </summary>
    public void OnDisable()
    {
        StopCoroutine(_startLocationService);
        _startLocationService = null;
        Debug.Log("Stop location services.");
        Input.location.Stop();
    }

    /// <summary>
    /// Unity's Update() method.
    /// </summary>
    public void Update()
    {
        // Check session error status.
        LifecycleUpdate();
        if (_isReturning)
        {
            return;
        }

        if (ARSession.state != ARSessionState.SessionInitializing &&
            ARSession.state != ARSessionState.SessionTracking)
        {
            return;
        }

        // Check feature support and enable Geospatial API when it's supported.
        var featureSupport = _arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                return;
            case FeatureSupported.Unsupported:
                ReturnWithReason("Geospatial API is not supported by this devices.");
                return;
            case FeatureSupported.Supported:
                if (_arCoreExtension.ARCoreExtensionsConfig.GeospatialMode ==
                    GeospatialMode.Disabled)
                {
                    Debug.Log("Geospatial switched to GeospatialMode.Enabled.");
                    _arCoreExtension.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    _configurePrepareTime = 3.0f;
                    _enablingGeospatial = true;
                    return;
                }

                break;
        }

        // Waiting for new configuration taking effect.
        if (_enablingGeospatial)
        {
            _configurePrepareTime -= Time.deltaTime;
            if (_configurePrepareTime < 0)
            {
                _enablingGeospatial = false;
            }
            else
            {
                return;
            }
        }

        // Check earth state.
        var earthState = _arEarthManager.EarthState;
        if (earthState == EarthState.ErrorEarthNotReady)
        {
            Debug.Log("Initializing Geospatial functionalities.");
            return;
        }
        else if (earthState != EarthState.Enabled)
        {
            Debug.Log("Geospatial sample encountered an EarthState error: " + earthState);
            return;
        }

        // Check earth localization.
        bool isSessionReady = ARSession.state == ARSessionState.SessionTracking &&
            Input.location.status == LocationServiceStatus.Running;
        //If the process can reach this line and isSessionReady is true, the GeospatialAPI is available
        _isReady = isSessionReady;

        if (!_isReady) return;

        var location_message = "";
        var pose = _arEarthManager.CameraGeospatialPose;
        if (_arEarthManager.EarthTrackingState == TrackingState.Tracking)
        {
            location_message = string.Format(
                "Latitude/Longitude: {0}°, {1}°\n" +
                "Horizontal Accuracy: {2}m\n" +
                "Altitude: {3}m\n" +
                "Vertical Accuracy: {4}m\n" +
                "Eun Rotation: {5}\n" +
                "Orientation Yaw Accuracy: {6}°\n",
                pose.Latitude.ToString(),
                pose.Longitude.ToString(),
                pose.HorizontalAccuracy.ToString(),
                pose.Altitude.ToString(),
                pose.VerticalAccuracy.ToString(),
                pose.EunRotation.ToString(),
                pose.OrientationYawAccuracy.ToString());
            FetchBussinessData();
        }
#region DEBUG
        var places_message = "";
        if (_places != null)
        {
            foreach (var place in _places.Places)
            {
                places_message += place.Name + "\t";
            }
        }
#endregion
        if (!isSessionReady || _arEarthManager.EarthTrackingState != TrackingState.Tracking ||
            _arEarthManager.CameraGeospatialPose.OrientationYawAccuracy > _orientationYawAccuracyThreshold ||
            _arEarthManager.CameraGeospatialPose.HorizontalAccuracy > _horizontalAccuracyThreshold)
        {
            _uiController.EnableLocalizationImage();
            _debugController.UpdatePanelMessage("Rotate around for buildings...\n" + location_message + places_message);
            return;
        }
        _uiController.DisableLocalizationImage();
        _debugController.UpdatePanelMessage("Success\n" + location_message + places_message);

        // set anchor
        if (_places != null)
        {
            _uiController.SpawnPlaces(_places);
        }
    }


    private void FetchBussinessData()
    {
        //fetch business data
        var currentPose = new Vector2((float)_arEarthManager.CameraGeospatialPose.Latitude, (float)_arEarthManager.CameraGeospatialPose.Longitude);
        try
        {

            if (TrackingState.Tracking == _arEarthManager.EarthTrackingState && _requestCounter == 0 &&
                ((_lastSavedPosition == Vector2.zero) || Location.FindDistance(currentPose.x, _lastSavedPosition.x, currentPose.y, _lastSavedPosition.y) < (_searchRadius / 2)))
            {
                Debug.Log("TaskStarted");

                if (placesTask != null) { placesTask.Dispose(); placesTask = null; }
                _requestCounter++;
                placesTask = Task.Run(() => BusinessData.GetPlaces(
                    _arEarthManager.CameraGeospatialPose.Latitude,
                    _arEarthManager.CameraGeospatialPose.Longitude,
                    (int)_searchRadius));
                _lastSavedPosition = currentPose;
            }

            if (placesTask != null && placesTask.IsCompleted)
            {
                Debug.Log("TaskCompleted");

                _places = placesTask.Result;
                this._task_finished = true;

                FindObjectsOfType<UIController>()[0].DespawnPlaces(_searchRadius, currentPose);
                FindObjectsOfType<UIController>()[0].SpawnPlaces(_places);
                placesTask = null;

                // Create Terrain Anchors
                foreach (var place in _places.Places)
                {
                    if (_arEarthManager.EarthState == EarthState.Enabled &&
                        _arEarthManager.EarthTrackingState == TrackingState.Tracking &&
                        !_anchorsHistory.Contains(place.Name)
                        )
                    {
                        var terrainAnchor = _arAnchorManager.ResolveAnchorOnTerrain(
                            place.Geometry.Location.Lat,
                            place.Geometry.Location.Lng,
                            0,
                            Quaternion.identity
                            );
                        terrainAnchor.gameObject.name = place.Name;
                        place._geoAnchor = terrainAnchor;
                        place._anchorInstantiated = false;
                        _anchorsHistory.Add(place.Name);
                    }
                }
            }

        }
        catch (Exception ex)
        {

            var obj = Instantiate(this._debuggerPrefab);
            obj.name = "exception" + ex.Message;
        }
    }

    private void LifecycleUpdate()
    {
        // Pressing 'back' button quits the app.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (_isReturning)
        {
            return;
        }

        // Only allow the screen to sleep when not tracking.
        var sleepTimeout = SleepTimeout.NeverSleep;
        if (ARSession.state != ARSessionState.SessionTracking)
        {
            sleepTimeout = SleepTimeout.SystemSetting;
        }

        Screen.sleepTimeout = sleepTimeout;

        // Quit the app if ARSession is in an error status.
        string returningReason = string.Empty;
        if (ARSession.state != ARSessionState.CheckingAvailability &&
            ARSession.state != ARSessionState.Ready &&
            ARSession.state != ARSessionState.SessionInitializing &&
            ARSession.state != ARSessionState.SessionTracking)
        {
            returningReason = string.Format(
                "Geospatial sample encountered an ARSession error state {0}.\n" +
                "Please start the app again.",
                ARSession.state);
        }
        else if (Input.location.status == LocationServiceStatus.Failed)
        {
            returningReason =
                "Geospatial sample failed to start location service.\n" +
                "Please start the app again and grant precise location permission.";
        }
        else if (_arCoreExtension == null)
        {
            returningReason = string.Format(
                "Geospatial sample failed with missing AR Components.");
        }

        ReturnWithReason(returningReason);
    }

    private void ReturnWithReason(string reason)
    {
        if (string.IsNullOrEmpty(reason))
        {
            return;
        }

        Debug.Log(reason);
        _isReturning = true;
        _isReady = false;
    }

    public void InstantiatePoseAnchor()
    {
        if (_arEarthManager.EarthTrackingState == TrackingState.Tracking)
        {
            var pose = _arEarthManager.CameraGeospatialPose;
            var anchor = _arAnchorManager.AddAnchor(pose.Latitude, pose.Longitude, pose.Altitude, pose.EunRotation);
            var anchorAsset = Instantiate(_debuggerPrefab, anchor.transform);
            var text = anchorAsset.GetComponent<TMP_Text>();
            text.text = "Debug anchor";
            Debug.Log("DEBUG: Instantiated a debug anchor: " + anchorAsset.name);
        }
    }
}
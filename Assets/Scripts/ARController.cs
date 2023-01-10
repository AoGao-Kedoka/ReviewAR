using Google.XR.ARCoreExtensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using UnityEngine.Android;

/// <summary>
/// THIS IS THE MAIN SCRIPT OF THE PROJECT!
/// </summary>
public class ARController : MonoBehaviour
{
    [Header("AR Properties")]
    [SerializeField] private ARSessionOrigin _arOrigin;
    [SerializeField] private ARSession _arSession;
    [SerializeField] private ARCoreExtensions _arExtensions;

    /// <summary>
    /// AR Anchor Manager from ARFoundation Extension
    /// </summary>
    [Header("Geospatial Properties")]
    [SerializeField] private ARAnchorManager _arAnchorManager;

    /// <summary>
    /// AR Raycast Manager from ARFoundation
    /// </summary>
    [SerializeField] private ARRaycastManager _arRaycastManager;

    /// <summary>
    /// AR Earth Manager from ARFoundation Extension
    /// </summary>
    [SerializeField] private AREarthManager _arEarthManager;

    /// <summary>
    /// radius for business search 
    /// </summary>
    [Header("Parameters")]
    [SerializeField] private float _searchRadius;

    /// <summary>
    /// Debug controller for logging, calling `_debugController.UpdateDebugMessage()` will
    /// update debug message on panel and log into logcat 
    /// </summary>
    [Header("Debug")]
    [SerializeField] private DebugController _debugController;

    /// <summary>
    /// UI controller for controlling all the UI
    /// </summary>
    [SerializeField] private UIController _uiController;

    [SerializeField] private GameObject _reviewPlaceHolder;

    /// <summary>
    /// orientation yaw accuracy threshold for checking whether camera helps with the localization
    /// </summary>
    private const double _orientationYawAccuracyThreshold = 25;

    /// <summary>
    /// horizontal accuracy threshold for checking whether camera helps with the localization
    /// </summary>
    private const double _horizontalAccuracyThreshold = 20;

    /// <summary>
    /// last saved latitude and longtitudevalue
    /// </summary>
    private Vector2 _lastSavedPosition = Vector2.zero;

    /// <summary>
    /// counter for business data call
    /// </summary>
    private int _businessDataCallCounter = 0;

    /// <summary>
    /// places got from the api requests
    /// </summary>
    private PlacesApiQueryResponse _places = null;

    //DEBUG ONLY
    public List<Place> pl2 = null;
    public GameObject DebuggerPrefab;

    /// <summary>
    /// indicate when async task has returned results, set it back to false when finished
    /// </summary>
    private bool _task_finished = false;
    Task<PlacesApiQueryResponse> placesTask = null;

    private void Awake()
    {
        // Lock app orientation
        // Screen.autorotateToLandscapeLeft = false;
        // Screen.autorotateToLandscapeRight = false;
        // Screen.autorotateToPortraitUpsideDown = false;
        // Screen.orientation = ScreenOrientation.Portrait;


        // Check AR game objects
        if (_arOrigin == null)
        {
            Debug.LogError("Cannot find ARSessionOrigin.");
        }

        if (_arSession == null)
        {
            Debug.LogError("Cannot find ARSession.");
        }

        if (_arExtensions == null)
        {
            Debug.LogError("Cannot find ARCoreExtensions.");
        }

        _debugController.UpdateDebugMessage("Awake finished");
    }

    private void OnEnable()
    {
        _arOrigin.gameObject.SetActive(true);
        _arSession.gameObject.SetActive(true);
        _arExtensions.gameObject.SetActive(true);

        StartCoroutine(StartLocationService());
    }

    private void Update()
    {
        LifecycleUpdate();
        bool enabled = EnableGeospatial();
        if (TrackingState.Tracking == _arEarthManager.EarthTrackingState)
        {
            enabled = true;
        }

        if (!enabled) return;

        //fetch business data
        var currentPose = new Vector2((float)_arEarthManager.CameraGeospatialPose.Latitude, (float)_arEarthManager.CameraGeospatialPose.Longitude);
        try
        {

            if (TrackingState.Tracking == _arEarthManager.EarthTrackingState && _businessDataCallCounter == 0 &&
                ((_lastSavedPosition == Vector2.zero) || Location.FindDistance(currentPose.x, _lastSavedPosition.x, currentPose.y, _lastSavedPosition.y) < 20))
            {
                var obj = Instantiate(this.DebuggerPrefab);
                obj.name = "TaskStarted";

                if(placesTask != null) { placesTask.Dispose(); placesTask = null; }
                _businessDataCallCounter++;
                placesTask = Task.Run(() => BusinessData.GetPlaces(currentPose.x, currentPose.y, (int)_searchRadius));
                _lastSavedPosition = currentPose;
            }

            if (placesTask != null && placesTask.IsCompleted)
            {
                var obj = Instantiate(this.DebuggerPrefab);
                obj.name = "TaskCompleted";

                _places = placesTask.Result;
                this._task_finished = true;
                pl2 = _places.Places;

                FindObjectsOfType<UIController>()[0].DespawnPlaces(_searchRadius, currentPose);
                FindObjectsOfType<UIController>()[0].SpawnPlaces(_places);
                placesTask = null;
            }

        }
        catch(Exception ex)
        {

            var obj = Instantiate(this.DebuggerPrefab);
            obj.name =  "exception" + ex.Message;


        }

        // AR overlay
        if (_places != null)
        {
            foreach (var place in _places.Places)
            {
            }
        }
    }


    /// <summary>
    /// Enable all the Geospatial functionality for our app
    /// </summary>
    /// <returns>True when sucess</returns>
    private bool EnableGeospatial()
    {
        // Enable Geospatial mode
        var featureSupport = _arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
            case FeatureSupported.Unknown:
                _debugController.UpdateDebugMessage("FeatureSupport unknown");
                return false;
            case FeatureSupported.Unsupported:
                _debugController.UpdateDebugMessage("FeatureSupport unsupported");
                return false;
            case FeatureSupported.Supported:
                _debugController.UpdateDebugMessage("FeatureSupport supported");
                if (_arExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                    GeospatialMode.Disabled)
                {
                    _debugController.UpdateDebugMessage("Geospatial sample switched to GeospatialMode.Enabled.");
                    _arExtensions.ARCoreExtensionsConfig.GeospatialMode =
                        GeospatialMode.Enabled;
                    return false;
                }
                break;
        }

        StartCoroutine(Waiter(3));

        // Enable earth state
        var earchState = _arEarthManager.EarthState;
        if (earchState == EarthState.ErrorEarthNotReady)
        {
            _debugController.UpdateDebugMessage("Earth not ready");
            return false;
        }
        else if (earchState != EarthState.Enabled)
        {
            _debugController.UpdateDebugMessage("Geospatial sample encountered an EarthState error: " + earchState);
            return false;
        }
        else
        {
            _debugController.UpdateDebugMessage("EarchState: " + earchState);
        }

        // Check earth localization
        bool isSessionReady = ARSession.state == ARSessionState.SessionTracking && Input.location.status == LocationServiceStatus.Running;
        var earthTrackingState = _arEarthManager.EarthTrackingState;
        var pose = earthTrackingState == TrackingState.Tracking ? _arEarthManager.CameraGeospatialPose : new GeospatialPose();
        string location_message = "";
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
        }
        if (!isSessionReady || earthTrackingState != TrackingState.Tracking ||
            pose.OrientationYawAccuracy > _orientationYawAccuracyThreshold ||
            pose.HorizontalAccuracy > _horizontalAccuracyThreshold)
        {
            _uiController.EnableLocalizationImage();
            _debugController.UpdateDebugMessage("Point camera at buildings, stores and signs near you\n" + location_message);
            return false;
        }

        _uiController.DisableLocalizationImage();
        _debugController.UpdateDebugMessage("Successfully Located\n" + location_message);
        return true;
    }
    private void LifecycleUpdate()
    {
        // Pressing 'back' button quits the app.
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
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
            _debugController.UpdateDebugMessage(
                    "Geospatial sample encountered an ARSession error state {0}.\n" +
                    "Please start the app again.");
        }
        else if (Input.location.status == LocationServiceStatus.Failed)
        {
            _debugController.UpdateDebugMessage(
                "Geospatial sample failed to start location service.\n" +
                "Please start the app again and grant precise location permission.");
        }
        else if (_arSession == null || _arOrigin == null || _arExtensions == null)
        {
            _debugController.UpdateDebugMessage(
                 "Geospatial sample failed with missing AR Components.");
        }
    }


    private IEnumerator StartLocationService()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            _debugController.AddDebugMessage("Requesting fine location permission.");
            Permission.RequestUserPermission(Permission.FineLocation);
            yield return new WaitForSeconds(3.0f);
        }

        if (!Input.location.isEnabledByUser)
        {
            _debugController.AddDebugMessage("Location service is disabled by User.");
            yield break;
        }

        _debugController.AddDebugMessage("Start location service.");
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            var message = string.Format("Location service ends with {0} status.", Input.location.status);
            _debugController.AddDebugMessage(message);
            Input.location.Stop();
        }
    }
    private IEnumerator Waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }
}

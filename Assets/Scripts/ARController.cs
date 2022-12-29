using Google.XR.ARCoreExtensions;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if UNTIY_ANDROID
    using UnityEngine.Android;
#endif

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
    /// Debug controller for logging, calling `_debugController.UpdateDebugMessage()` will
    /// update debug message on panel and log into logcat 
    /// </summary>
    [Header("Debug")]
    [SerializeField] private DebugController _debugController;

    /// <summary>
    /// UI controller for controlling all the UI
    /// </summary>
    [SerializeField] private UIController _uiController;

    /// <summary>
    /// orientation yaw accuracy threshold for checking whether camera helps with the localization
    /// </summary>
    private const double _orientationYawAccuracyThreshold = 25;

    /// <summary>
    /// horizontal accuracy threshold for checking whether camera helps with the localization
    /// </summary>
    private const double _horizontalAccuracyThreshold = 20;

    private void Awake()
    {
        // Lock app orientation
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;


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
        StartCoroutine(StartLocationService());

        _arOrigin.gameObject.SetActive(true);
        _arSession.gameObject.SetActive(true);
        _arExtensions.gameObject.SetActive(true);
    }

    void Update()
    {
        bool enabled = EnableGeospatial();
        if (!enabled) return;

        // fetch business data
        // Task<PlacesApiQueryResponse> reviews = null;
        // if (TrackingState.Tracking == _arEarthManager.EarthTrackingState)
        //     reviews = BusinessData.GetPlaces((float)_arEarthManager.CameraGeospatialPose.Latitude, (float)_arEarthManager.CameraGeospatialPose.Longitude);

        // TODO: Calculate reviews location
        // if (reviews == null) { return; }
        // List<Place> places = reviews.Result.Places;
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
        return true;
    }

    private IEnumerator StartLocationService()
    {
#if UNTIY_ANDROID
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
            yield break;
        }

        Debug.Log("Start location service.");
        Input.location.Start();

        while (Input.location.status == LocationServiceStatus.Initializing)
        {
            yield return null;
        }

        if (Input.location.status != LocationServiceStatus.Running)
        {
            Debug.LogWarningFormat(
                "Location service ends with {0} status.", Input.location.status);
            Input.location.Stop();
        }
    }
    private IEnumerator Waiter(float time)
    {
        yield return new WaitForSeconds(time);
    }
}

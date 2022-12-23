using Google.XR.ARCoreExtensions;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using TMPro;

#if UNTIY_ANDROID
    using UnityEngine.Android;
#endif

public class ARController : MonoBehaviour
{
    [Header("AR Properties")]
    [SerializeField] private ARSessionOrigin    _arOrigin;
    [SerializeField] private ARSession          _arSession;
    [SerializeField] private ARCoreExtensions   _arExtensions;

    [Header("Geospatial Properties")]
    [SerializeField] private ARAnchorManager    _arAnchorManager;
    [SerializeField] private ARRaycastManager   _arRaycastManager;
    [SerializeField] private AREarthManager     _arEarthManager;

    [Header("Debug")]
    [SerializeField] private DebugController    _debugController; 
    
    private void Awake()
    {
        // Lock app orientation
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;


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
        // Enable Geospatial mode
        var featureSupport = _arEarthManager.IsGeospatialModeSupported(GeospatialMode.Enabled);
        switch (featureSupport)
        {
                case FeatureSupported.Unknown:
                    return;
                case FeatureSupported.Unsupported:
                    return;
                case FeatureSupported.Supported:
                    if (_arExtensions.ARCoreExtensionsConfig.GeospatialMode ==
                        GeospatialMode.Disabled)
                    {
                        Debug.Log("Geospatial sample switched to GeospatialMode.Enabled.");
                        _arExtensions.ARCoreExtensionsConfig.GeospatialMode =
                            GeospatialMode.Enabled;
                        return;
                    }
                    break;
        }

        StartCoroutine(Waiter(3));

        // Enable earth state
        var earchState = _arEarthManager.EarthState;
        if (earchState == EarthState.ErrorEarthNotReady)
        {
            return;
        }
        else if (earchState != EarthState.Enabled)
        {
            _debugController.UpdateDebugMessage("Geospatial sample encountered an EarthState error: " + earchState);
            return;
        }
        else
        {
            _debugController.UpdateDebugMessage("EarchState: " + earchState);
        }
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

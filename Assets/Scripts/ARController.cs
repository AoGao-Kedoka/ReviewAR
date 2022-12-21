using Google.XR.ARCoreExtensions;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

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

    private bool _waitingForLocationService = false;
    
    private void Awake()
    {
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.orientation = ScreenOrientation.Portrait;


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

    }
    private IEnumerator StartLocationService()
    {
        _waitingForLocationService = true;

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
}

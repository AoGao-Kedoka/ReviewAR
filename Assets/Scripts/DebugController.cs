using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MarkupAttributes;
using Google.XR.ARCoreExtensions;
using UnityEngine.XR.ARFoundation;

#if UNITY_EDITOR
using UnityEditor;
using MarkupAttributes.Editor;

[CustomEditor(typeof(DebugController)), CanEditMultipleObjects]
internal class DebugControllerEditor : MarkedUpEditor {}
#endif

public class DebugController : MonoBehaviour
{
    [Box("Properties")]
    [SerializeField] private bool _active = true;
    [SerializeField] public float _searchRadius;
    [EndGroup("Properties")]

    [Box("Debug Objects")]
    [ShowIf(nameof(_active))]
    [SerializeField] public bool _lockScreenToPortrait = true;

    [Tooltip("Debugging objects that should be handled by this sript")]
    [ShowIf(nameof(_active))]
    [SerializeField] private List<GameObject> _debugObjects;

    [Header("Other gameobjects with custom behavior")]
    [ShowIf(nameof(_active))]
    [SerializeField] private TMP_Text _debugText;
    [ShowIf(nameof(_active))]
    [SerializeField] public GameObject _debuggerPrefab;
    [ShowIf(nameof(_active))]
    [SerializeField] public int _indexArrowShows;
    [ShowIf(nameof(_active))]
    [SerializeField] public GameObject _arrow;
    [ShowIf(nameof(_active))]
    [Header("Create Prefab with Button Click")]
    [SerializeField] public Button _createAnchorButton;
    [ShowIf(nameof(_active))]
    [SerializeField] public GameObject _buttonCreatedPrefabs;
    [EndGroup("Debug Objects")]

    private AREarthManager _arEarthManager;
    private ARAnchorManager _arAnchorManager;

    private void Start()
    {
        foreach (var obj in _debugObjects)
        {
            obj?.SetActive(_active);
        }
        if (_active)
        {
            _arEarthManager = FindObjectOfType<AREarthManager>();
            _arAnchorManager = FindObjectOfType<ARAnchorManager>();
            _createAnchorButton.onClick.AddListener(InstantiatePoseAnchor);
        }
    }
    /// <summary>
    /// clear panel and update debug message on the panel
    /// </summary>
    /// <param name="message"></param>
    public void UpdatePanelMessage(string message)
    {
        _debugText.text = message;
    }

    /// <summary>
    /// add debug message on the panel without clearing the panel messages
    /// </summary>
    /// <param name="message"></param>
    public void AddMessageToPanel(string message)
    {
        _debugText.text += message;
    }
        
    /// <summary>
    /// Button behaviour when onclick
    /// A _buttonCreatedPrefab will generated at current camera's pose
    /// </summary>
    public void InstantiatePoseAnchor()
    {
        if (_arEarthManager.EarthTrackingState == UnityEngine.XR.ARSubsystems.TrackingState.Tracking)
        {
            var pose = _arEarthManager.CameraGeospatialPose;
            var anchor = _arAnchorManager.AddAnchor(pose.Latitude, pose.Longitude, pose.Altitude, pose.EunRotation);
            var anchorAsset = Instantiate(_buttonCreatedPrefabs, anchor.transform);
            var text = anchorAsset.GetComponent<TMP_Text>();
            text.text = "Debug anchor";
            Debug.Log("DEBUG: Instantiated a debug anchor: " + anchorAsset.name);
        }
    }
}

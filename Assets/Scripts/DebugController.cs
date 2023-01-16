using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using MarkupAttributes;

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

    [TitleGroup("./Other gameobjects with custom behavior")]
    [ShowIf(nameof(_active))]
    [SerializeField] private TMP_Text _debugText;
    [ShowIf(nameof(_active))]
    [SerializeField] public GameObject _debuggerPrefab;
    [ShowIf(nameof(_active))]
    [SerializeField] public int _indexArrowShows;
    [ShowIf(nameof(_active))]
    [SerializeField] public GameObject _arrow;

    private void Start()
    {
        foreach (var obj in _debugObjects)
        {
            obj?.SetActive(_active);
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
}

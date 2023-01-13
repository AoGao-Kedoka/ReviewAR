using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.XR.ARFoundation;

public class DebugController : MonoBehaviour
{
    private bool active = true;
    [Tooltip("Debugging objects that should be handled by this sript")]
    [SerializeField] private List<GameObject> _debugObjects;
    [SerializeField] private TMP_Text _debugText;

    public void ToogleDebug()
    {
        foreach (GameObject g in _debugObjects)
        {
            g.SetActive(!active);
        }
        active = !active;
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

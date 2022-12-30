using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugController : MonoBehaviour
{
    private bool active = true;
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
    public void UpdateDebugMessage(string message)
    {
        _debugText.text = message;
        Debug.Log(message);
    }

    /// <summary>
    /// add debug message on the panel without clearing the panel messages
    /// </summary>
    /// <param name="message"></param>
    public void AddDebugMessage(string message)
    {
        _debugText.text += message;
        Debug.Log(message);
    }
}

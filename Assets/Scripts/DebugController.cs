using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugController : MonoBehaviour
{
    private bool active = false;
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

    public void UpdateDebugMessage(string message)
    {
        if (active)
            _debugText.text += message;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugController : MonoBehaviour
{
    private bool active = false;
    [SerializeField] List<GameObject> _debugObjects;

    public void ToogleDebug()
    {
        foreach (GameObject g in _debugObjects)
        {
            g.SetActive(!active);
        }
        active = !active;
    }
}

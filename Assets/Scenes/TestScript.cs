using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public float width = 2.0f;
    // Start is called before the first frame update
    void Start()
    {
    }
    [ContextMenu("Test")]
    public void test()
    {
        this.GetComponentInChildren<RectTransform>().sizeDelta = this.GetComponentInChildren<RectTransform>().sizeDelta * 2;
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

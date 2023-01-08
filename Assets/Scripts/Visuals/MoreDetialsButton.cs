using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreDetialsButton : MonoBehaviour
{

    public string id;
    public string bname;
    private string apiKey;
    public void OnClicked()
    {
        //Application.OpenURL(string.Format("https://maps.googleapis.com/maps/api/place/details/json?place_id={0}&key={1}", this.id, this.apiKey));
        Application.OpenURL(string.Format("https://www.google.com/maps/place/{1}/?qplace_id={0}", this.id, string.Join('+', this.bname.Split(" "))));
    

    }

    private void OnEnable()
    {
        // Load config file
        TextAsset file = Resources.Load<TextAsset>("Keys");
        if (file != null)
        {
            apiKey = file.text.Split('=')[1];
            Debug.Log("Key: " + apiKey);

        }
        else
        {
            Application.Quit();
        }
    }
}

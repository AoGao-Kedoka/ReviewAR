using UnityEngine;
using System.Web;

public class MoreDetialsButton : MonoBehaviour
{
    public string id;
    public string bname;
    public void OnClicked()
    {
        Application.OpenURL(string.Format("https://www.google.com/maps/search/?api=1&query={0}&query_place_id={1}", HttpUtility.UrlEncode(this.bname), this.id));
    }
}

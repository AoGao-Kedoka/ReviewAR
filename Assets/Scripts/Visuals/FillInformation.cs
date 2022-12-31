using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FillInformation : MonoBehaviour
{
    private GameObject MetadataPanel;
    private GameObject ReviewsPanel;


    /// <summary>
    /// Fills the info of panel with business details
    /// </summary>
    public void FillInfo(Place place)
    {
        if (place.Name is not null)
        {
            GameObject.Find("BusinessName").GetComponent<TextMeshProUGUI>().text = place.Name;
        }
        else
        {
            GameObject.Find("BusinessName").SetActive(false);
        }

        if (place.Rating.HasValue)
        {
            GameObject.Find("ReviewsScore").GetComponent<TextMeshProUGUI>().text = place.Rating.Value.ToString();
            GameObject.Find("ColorImageS").GetComponent<RectTransform>()
                                          .sizeDelta
                                          .Scale((new Vector3(1, place.Rating.Value / 5.0f)));
        }
        else
        {
            GameObject.Find("ReviewsSection").SetActive(false);
        }

        if (place.Price_level.HasValue)
        {
            GameObject.Find("PricesScore").GetComponent<TextMeshProUGUI>().text = place.Price_level.Value.ToString();
            GameObject.Find("ColorImageP").GetComponent<RectTransform>()
                                          .sizeDelta.Scale((new Vector3(1, place.Price_level.Value / 4.0f)));

        }
        else
        {
            GameObject.Find("PricesSection").SetActive(false);
        }

        if (place.OpeningHours is not null)
        {
            GameObject.Find("OpenStatus").GetComponent<TextMeshProUGUI>().text = (place.OpeningHours.Open_now == true ? "Open" : "Closed");
            GameObject.Find("OpenStatus").GetComponent<TextMeshProUGUI>().color = (place.OpeningHours.Open_now == true ? Color.green : Color.red);

        }
        else
        {
            GameObject.Find("OpenStatus").SetActive(false);
        }

        if (place.Place_id is not null)
        {
            GameObject.Find("ExternalButton").GetComponent<MoreDetialsButton>().id = place.Place_id;
        }
        else
        {
            GameObject.Find("ExternalButton").SetActive(false);
        }

        if (place.Reviews.Count > 0)
        {
            GameObject.Find("ReviewExpandButton").SetActive(false);
        }
        else//TODO: finish the image logic
        {
            int i = 0;
            for (; i < place.Reviews.Count; ++i)
            {
                GameObject.Find(string.Format("ReviewText{0}", i)).GetComponent<TextMeshProUGUI>().text = place.Reviews[i].Text;
                GameObject.Find("ColorImage" + i).GetComponent<RectTransform>()
                           .sizeDelta.Scale((new Vector3(1, place.Price_level.Value / 5.0f)));
            }
            for(; i < 3; ++i)
            {
                GameObject.Find(string.Format("ReviewText{0}", i)).SetActive(false);
                GameObject.Find(string.Format("ReviewImage{0}", i)).SetActive(false);
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FillInformation : MonoBehaviour
{
    private GameObject MetadataPanel;
    private GameObject ReviewsPanel;
    public Location PlaceLocation;//how to find distance

    /// <summary>
    /// Fills the info of panel with business details
    /// </summary>
    public void FillInfo(Place place)
    {
        if (place.Name is not null)
        {
            this.transform.Find("MetadataPanel/BusinessName").GetComponent<TextMeshProUGUI>().text = place.Name;
        }
        else
        {
            this.transform.Find("MetadataPanel/BusinessName").gameObject.SetActive(false);
        }

        if (place.Rating.HasValue)
        {
            this.transform.Find("MetadataPanel/ReviewsSection/ReviewsScore").GetComponent<TextMeshProUGUI>().text = place.Rating.Value.ToString();
            var RTComponent = this.transform.Find("MetadataPanel/ReviewsSection/ReviewScoreImages/ColorImageS").GetComponent<RectTransform>();
            RTComponent.sizeDelta = new Vector2(RTComponent.sizeDelta.x * (place.Rating.Value / 5.0f), RTComponent.sizeDelta.y);

        }
        else
        {
            this.transform.Find("MetadataPanel/ReviewsSection").gameObject.SetActive(false);
        }

        if (place.Price_level.HasValue)
        {
            this.transform.Find("MetadataPanel/PriceSection/PricesScore").GetComponent<TextMeshProUGUI>().text = place.Price_level.Value.ToString();
            var RTComponent = this.transform.Find("MetadataPanel/PriceSection/PriceImages/ColorImageP").GetComponent<RectTransform>();
            RTComponent.sizeDelta = new Vector2(RTComponent.sizeDelta.x * (place.Price_level.Value / 4.0f), RTComponent.sizeDelta.y);

        }
        else
        {
            this.transform.Find("MetadataPanel/PriceSection").gameObject.SetActive(false);
        }

        if (place.OpeningHours is not null)
        {
            this.transform.Find("MetadataPanel/OpenStatus").GetComponent<TextMeshProUGUI>().text = (place.OpeningHours.Open_now == true ? "Open" : "Closed");
            this.transform.Find("MetadataPanel/OpenStatus").GetComponent<TextMeshProUGUI>().color = (place.OpeningHours.Open_now == true ? Color.green : Color.red);

        }
        else
        {
            this.transform.Find("MetadataPanel/OpenStatus").gameObject.SetActive(false);
        }

        if (place.Place_id is not null && place.Name is not null)
        {
            this.transform.Find("MetadataPanel/ExternalButton").GetComponent<MoreDetialsButton>().id = place.Place_id;
            this.transform.Find("MetadataPanel/ExternalButton").GetComponent<MoreDetialsButton>().bname = place.Name;
        }
        else
        {
            this.transform.Find("MetadataPanel/ExternalButton").gameObject.SetActive(false);
        }

        if (place.Reviews?.Count == 0)
        {
            this.transform.Find("MetadataPanel/ReviewsSection/ReviewExpandButton").gameObject.SetActive(false);
        }
        else//TODO: finish the image logic
        {
            this.transform.Find("MetadataPanel/ReviewsSection/ReviewExpandButton").GetComponent<ExpandButton>().ButtonPressed();

            int i = 1;
            Debug.Log("cout:");

            Debug.Log(place.Reviews?.Count);
            for (; i <= place.Reviews?.Count; ++i)
            {
                this.transform.Find(string.Format("ReviewsPanel/Review{0}/ReviewText{0}", i)).GetComponent<TextMeshProUGUI>().text = place.Reviews[i].Text;
                var RTComponent = this.transform.Find(string.Format("ReviewsPanel/Review{0}/ColorImage{0}", i)).GetComponent<RectTransform>();
                RTComponent.sizeDelta = new Vector2(RTComponent.sizeDelta.x * (place.Rating.Value / 5.0f), RTComponent.sizeDelta.y);
            }
            for (; i <= 3; ++i)
            {
                this.transform.Find(string.Format("ReviewsPanel/Review{0}", i)).gameObject.SetActive(false);
               //GameObject.Find(string.Format("ReviewImage{0}", i)).SetActive(false);
            }

        }
        this.PlaceLocation = place.Geometry.Location;
    }
}

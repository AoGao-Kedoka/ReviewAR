using System.Threading;
using TMPro;
using UnityEngine;

public class FillInformation : MonoBehaviour
{
    public Location PlaceLocation;//how to find distance
    public static Mutex mut = new Mutex();
    public  Place UpdatedPlace = null;
    private bool CalledOnce = false;

    GameObject ReviewExpandButton;
    GameObject[] ReviewObjects;
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

        if (place.Reviews == null || place.Reviews.Count == 0)
        {
            mut.WaitOne();

            this.ReviewExpandButton.GetComponent<ExpandButton>().Disable();
            this.ReviewExpandButton.gameObject.SetActive(false);
            mut.ReleaseMutex();

        }
        else//TODO: finish the image logic
        {
            mut.WaitOne();
            if (!this.CalledOnce)
            {
                FillReviews(place);
            }
            mut.ReleaseMutex();

        }
        this.PlaceLocation = place.Geometry.Location;
    }

    public void FillReviews(Place place)
    {
        this.ReviewExpandButton.SetActive(true);

        // dont remember why i had this
        this.ReviewExpandButton.GetComponent<ExpandButton>().Enable();

        int i = 1;
        Debug.Log("cout:");

        Debug.Log(place.Reviews?.Count);
        for (; i <= place.Reviews?.Count && i <= 3; ++i)
        {
           this.ReviewObjects[i-1].GetComponentInChildren<TextMeshProUGUI>().text = place.Reviews[i-1].Text;

            var RTComponent = this.ReviewObjects[i - 1].transform.Find(string.Format("ReviewImage{0}/ColorImage{0}", i)).GetComponent<RectTransform>();
            RTComponent.sizeDelta = new Vector2(RTComponent.sizeDelta.x * (place.Reviews[i - 1].Rating / 5.0f), RTComponent.sizeDelta.y);
        }
        for (; i <= 3; ++i)
        {
            this.ReviewObjects[i-1].SetActive(false);
        }
    }
    private void Update()
    {

        mut.WaitOne();
        if (UpdatedPlace != null && !CalledOnce)
        {
            CalledOnce = true;
            FillReviews(UpdatedPlace);
            UpdatedPlace = null;
        }
        mut.ReleaseMutex();
    }

    public void init()
    {
        this.UpdatedPlace = null;
        this.ReviewExpandButton = this.transform.Find("MetadataPanel/ReviewsSection/ReviewExpandButton").gameObject;
        this.ReviewObjects = new GameObject[3];
        for(int i = 1; i <= 3; ++i)
        {
            this.ReviewObjects[i - 1] = this.transform.Find(string.Format("ReviewsPanel/Review{0}", i)).gameObject;
        }
    }
}


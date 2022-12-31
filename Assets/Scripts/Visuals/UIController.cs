using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ElRaccoone.Tweens;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image _localizingImage;
    [SerializeField] GameObject InformationCanvas;
    /// <summary>
    /// Enable localizing icon in the center of screen
    /// </summary>
    public void EnableLocalizationImage()
    {
        if (!_localizingImage.gameObject.activeSelf)
        {
            _localizingImage.gameObject.SetActive(true);    
            _localizingImage.gameObject.TweenAnchoredPositionY(gameObject.transform.localPosition.y + 100, 1).SetPingPong().SetInfinite();
        }
    }
    
    /// <summary>
    /// Disable localizing icon
    /// </summary>
    public void DisableLocalizationImage()
    {
        _localizingImage.gameObject.SetActive(false);
    }


    // TODO: need orientation from ao
    //       also need to clarify who holds the objects
    /// <summary>
    /// Spawns all the panels
    /// </summary>
    public void SpawnPlaces(PlacesApiQueryResponse places) {
        foreach (Place place in places.Places) {
            var obj = Instantiate(this.InformationCanvas);
            obj.transform.position = place.position;
            obj.GetComponent<FillInformation>().FillInfo(place);
        }
    }

}

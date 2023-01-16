using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ElRaccoone.Tweens;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image _localizingImage;
    [SerializeField] GameObject InformationCanvas;
    private List<GameObject> SpawnedPanels = new List<GameObject>();

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
            // Check terrain anchor state
            var terrainAnchor = place._terrainAnchor;
            if (terrainAnchor == null || 
                terrainAnchor.terrainAnchorState != Google.XR.ARCoreExtensions.TerrainAnchorState.Success)
            {
                if (terrainAnchor == null)
                {
                    Debug.LogError($"Terrain Anchor is null");
                    return;
                }
                if (terrainAnchor.terrainAnchorState != Google.XR.ARCoreExtensions.TerrainAnchorState.TaskInProgress)
                    Debug.LogError($"Terrain Anchor encountered an error: {terrainAnchor.terrainAnchorState}");
                return; 
            }
            Debug.Log($"Anchor {place._terrainAnchor.name} created successfully");
            
            // Instantiate panel 
            if (!place._anchorInstantiated)
            {
                var obj = Instantiate(this.InformationCanvas, place._terrainAnchor?.transform);
                obj.GetComponent<FillInformation>().FillInfo(place);
                SpawnedPanels.Add(obj);
            }
        }
    }

    public void DespawnPlaces(float radius, Vector2 currentLocation)
    {

        foreach (GameObject canvas in this.SpawnedPanels)
        {
            if (Location.FindDistance(canvas.GetComponent<FillInformation>().PlaceLocation.Lat,
                                      currentLocation.x,
                                      canvas.GetComponent<FillInformation>().PlaceLocation.Lng,
                                      currentLocation.y
                                      ) > radius){ 
                this.SpawnedPanels.Remove(canvas);
                Destroy(canvas.gameObject);
            }
        }
    }

}

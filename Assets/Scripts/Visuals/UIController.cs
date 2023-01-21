using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ElRaccoone.Tweens;
using System.Threading.Tasks;

public class UIController : MonoBehaviour
{
    [SerializeField] private Image _localizingImage;
    [SerializeField] GameObject InformationCanvas;
    [SerializeField] private GameObject _playerCam;
    public Dictionary<string, GameObject> SpawnedPanels = new Dictionary<string, GameObject>();

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


    /// <summary>
    /// Spawns all the panels
    /// </summary>
    public void SpawnPlaces(PlacesApiQueryResponse places) {
        foreach (Place place in places.Places) {
            if (!this.SpawnedPanels.ContainsKey(place.Name))
            {

                // Check terrain anchor state
                  var terrainAnchor = place._terrainAnchor;
            if (terrainAnchor == null || 
                terrainAnchor.terrainAnchorState != Google.XR.ARCoreExtensions.TerrainAnchorState.Success)
            {
                if (terrainAnchor == null)
                {
                    Debug.LogError($"Terrain Anchor is null for place {place.Name}");
                    continue;
                }
                if (terrainAnchor.terrainAnchorState != Google.XR.ARCoreExtensions.TerrainAnchorState.TaskInProgress)
                    Debug.LogError($"Terrain Anchor encountered an error: {terrainAnchor.terrainAnchorState} for place {place.Name}");
                    continue;
                }

                // Instantiate panel 
                if (!place._anchorInstantiated)
                {
                    Debug.Log($"Anchor {place._terrainAnchor.name} created successfully");
                    var obj = Instantiate(this.InformationCanvas,  place._terrainAnchor?.transform);
                    obj.GetComponent<FillInformation>().init();
                    obj.GetComponent<FillInformation>().FillInfo(place);
                    SpawnedPanels.Add(place.Name, obj);
                    FillInformation fillComponent = obj.GetComponent<FillInformation>();
                    Task.Run(async () => await BusinessData.GetFullPlace(place))
                                                .ContinueWith((result) =>
                                                {
                                                    place.Reviews = result.Result.Reviews;
                                                    var panelobj = this.SpawnedPanels[place.Name];
                                                    if (panelobj != null)
                                                    {
                                                        FillInformation.mut.WaitOne();
                                                        fillComponent.UpdatedPlace = place;
                                                        FillInformation.mut.ReleaseMutex();
                                                    }
                                                }
                                               );
                }
            }
        }
    }

    public void DespawnPlaces(float radius, Vector2 currentLocation)
    {

        foreach (var item in this.SpawnedPanels)
        {
            if (Location.FindDistance(item.Value.GetComponent<FillInformation>().PlaceLocation.Lat,
                                      currentLocation.x,
                                      item.Value.GetComponent<FillInformation>().PlaceLocation.Lng,
                                      currentLocation.y
                                      ) > radius){ 
                this.SpawnedPanels.Remove(item.Key);
                Destroy(item.Value.gameObject);
            }
        }
    }

}

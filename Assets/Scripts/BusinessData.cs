using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(BusinessData))]
public class ResourceChecker : Editor
{
    public override void OnInspectorGUI()
    {
        BusinessData business = (BusinessData)target;
        TextAsset file = Resources.Load<TextAsset>("Keys");
        if (file == null)
            Debug.LogWarning("No config file found");
    }
    
}
#endif

public class BusinessData : MonoBehaviour
{

    private static string apiKey = null;

    private static HttpClient httpClient = new HttpClient();
    Task<PlacesApiQueryResponse> placesTask = null;
    private bool _task_finished = false;

    private PlacesApiQueryResponse _places = null;
    private bool done = false;

    // Example usage:
    // main(){
    //  var reviews = GetPlaces(-10.02, 30.1).Result; 
    // }
    static async public Task<PlacesApiQueryResponse> GetPlaces(double latitude, double longitude, int radius)
    {
        if (apiKey == null) { Application.Quit(); }

        using HttpResponseMessage response = await BusinessData.httpClient.GetAsync(String.Format
            ("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0}%2C{1}&radius={2}&key={3}"
            , latitude, longitude, radius, apiKey)).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result_text = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PlacesApiQueryResponse>(result_text);

    }
    static async public Task<Place> GetFullPlace(Place place)
    {
        if (apiKey == null) { Application.Quit(); }

        using HttpResponseMessage response = await BusinessData.httpClient.GetAsync(String.Format
            ("https://maps.googleapis.com/maps/api/place/details/json?place_id={0}&key={1}"
            , place.Place_id, apiKey)).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var result_text = await response.Content.ReadAsStringAsync();
        var t = JsonConvert.DeserializeObject<PlacesApiQueryResponse>(result_text);
        return t.Place;
    }

    [ContextMenu("Test")]
    public  async void Call()
    {
        //PlacesApiQueryResponse reviews = GetPlaces(48.1098956f, 11.4768243f, 1000).Result;

        //FindObjectsOfType<UIController>()[0].DespawnPlaces(40, new Vector2(10, 11));
        //FindObjectsOfType<UIController>()[0].SpawnPlaces(reviews);
        //Debug.Log(httpClient);
        //Debug.Log(reviews);

        
        {

            if (placesTask == null && !done)
            {
                Debug.Log("TaskStarted");

                if (placesTask != null) { placesTask.Dispose(); placesTask = null; }
                placesTask = Task.Run(() => BusinessData.GetPlaces(48.1098956f, 11.4768243f, 1000));
            }

            if (placesTask != null && placesTask.IsCompleted)
            {
                Debug.Log("TaskCompleted");
                done = true;
                _places = placesTask.Result;
                this._task_finished = true;

                placesTask = null;

                FindObjectsOfType<UIController>()[0].DespawnPlaces(40, new Vector2(10, 11));
                FindObjectsOfType<UIController>()[0].SpawnPlaces(_places);
                var t = FindObjectsOfType<UIController>()[0];
                // Create Terrain Anchors
                foreach (Place place in _places.Places)
                {
                    FillInformation fillComponent = t.SpawnedPanels[place.Name].GetComponent<FillInformation>();

                    Task.Run(async ()=> await BusinessData.GetFullPlace(place))
                                               .ContinueWith((result) => {
                                                   place.Reviews = result.Result.Reviews;
                                                   var panelobj = t.SpawnedPanels[place.Name];
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

    private void OnEnable()
    {
        // Load config file
        TextAsset file = Resources.Load<TextAsset>("Keys");
        if (file != null)
        {
            apiKey = file.text.Split('=')[1];
            Debug.Log("Key: " + apiKey);
        }
        else {
            Application.Quit();
        }
    }
}

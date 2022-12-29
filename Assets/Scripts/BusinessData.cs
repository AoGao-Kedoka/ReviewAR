using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

public class BusinessData : MonoBehaviour
{

    private static string apiKey = null;

    private static HttpClient httpClient = new HttpClient();

    // Example usage:
    // main(){
    //  var reviews = GetPlaces(-10.02, 30.1).Result; 
    // }
    static async public Task<PlacesApiQueryResponse> GetPlaces(float latitude, float longitude, int radius)
    {
        if (apiKey == null) { Application.Quit(); }

        using HttpResponseMessage response = await BusinessData.httpClient.GetAsync(String.Format
            ("https://maps.googleapis.com/maps/api/place/nearbysearch/json?location={0}%2C{1}&radius={2}&key={3}"
            , latitude, longitude, radius, apiKey)).ConfigureAwait(false);

        response.EnsureSuccessStatusCode();
        var result_text = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<PlacesApiQueryResponse>(result_text);

    }

    [ContextMenu("Test")]
    public  void Call()
    {
        PlacesApiQueryResponse reviews = GetPlaces(10f, 11f, 10000).Result;
        Debug.Log(httpClient);
        Debug.Log(reviews);
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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
public class BusinessData : MonoBehaviour
{

    private static string apiKey = "";

    private static HttpClient httpClient = new ()
    {
        BaseAddress = new System.Uri("https://maps.googleapis.com/maps/api/place"),
    };

    // Example usage:
    // main(){
    //  var reviews = GetPlaces(-10.02, 30.1).Reuslt(); 
    // }
    static async Task<PlacesApiQueryResponse> GetPlaces(float latitude, float longitude)
    {
        using HttpResponseMessage response = await BusinessData.httpClient.GetAsync(String.Format
            ("nearbysearch/json?location={0}%2C{1}&key={2}"
            ,latitude, longitude, apiKey));

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<PlacesApiQueryResponse>(await response.Content.ReadAsStringAsync());

    }
    private void Awake()
    {
        TextAsset file = Resources.Load<TextAsset>("Keys");
        if (file != null)
        {
            Debug.Log(file.text.Split('=')[1]);
        }
        else {
            Application.Quit();
        }
    }
}
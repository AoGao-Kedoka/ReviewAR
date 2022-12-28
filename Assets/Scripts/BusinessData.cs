using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using UnityEngine;
public class BussinessData : MonoBehaviour
{
    static string apiKey = "API_KEY";

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
        using HttpResponseMessage response = await BussinessData.httpClient.GetAsync(String.Format
            ("nearbysearch/json?location={0}%2C{1}&key={2}"
            ,latitude, longitude, apiKey));

        response.EnsureSuccessStatusCode();

        return JsonConvert.DeserializeObject<PlacesApiQueryResponse>(await response.Content.ReadAsStringAsync());

    }

}

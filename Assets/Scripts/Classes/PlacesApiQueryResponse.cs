using Newtonsoft.Json;
using System.Collections.Generic;

public class PlacesApiQueryResponse
{
    /// <summary>
    /// All the places found in proximity
    /// </summary>
    [JsonProperty("results")]
    public List<Place> Places { get; set; }
    /// <summary>
    /// When requesting single place details
    /// </summary>
    [JsonProperty("result")]
    public Place Place { get; set; }
}
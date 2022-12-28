using Newtonsoft.Json;
using System.Collections.Generic;

public class PlacesApiQueryResponse
{
    /// <summary>
    /// All the places found in proximity
    /// </summary>
    [JsonProperty("results")]
    public List<Place> Places { get; }
}
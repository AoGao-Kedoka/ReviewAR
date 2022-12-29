using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location
{
    /// <summary>
    /// Location latitude,  optional
    /// </summary>
    [JsonProperty("lat")]
    public float Lat { get; set; }

    /// <summary>
    /// Location longitutde,  optional
    /// </summary>
    [JsonProperty("lng")]
    public float Lng { get; set; }
}

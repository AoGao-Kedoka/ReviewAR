using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport
{
    /// <summary>
    /// Location latitude,  optional
    /// </summary>
    [JsonProperty("northeast")]
    public Location NE { get; set; }

    /// <summary>
    /// Location longitutde,  optional
    /// </summary>
    [JsonProperty("southwest")]
    public Location SW { get; set; }
}

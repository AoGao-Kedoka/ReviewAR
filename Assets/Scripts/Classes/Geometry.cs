using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class Geometry
{

    /// <summary>
    /// Location optional
    /// </summary>
    [JsonProperty("location")]
    public Location Location { get; set; }

    /// <summary>
    /// viewport optional
    /// </summary>
    [JsonProperty("viewport")]
    public Viewport Viewport { get; set; }

}

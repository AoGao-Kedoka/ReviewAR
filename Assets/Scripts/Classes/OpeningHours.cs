using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpeningHours
{
    /// <summary>
    /// Open now optional
    /// </summary>
    [JsonProperty("open_now")]
    public bool Open_now { get; set; }

}

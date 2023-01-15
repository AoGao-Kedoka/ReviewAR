using Google.XR.ARCoreExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

#nullable enable
[System.Serializable]
public class Place
{
    /// <summary>
    /// Business name. eg: "Sergio Tapia, John Cosack, Lucy McMillan", optional
    /// </summary>
    [JsonProperty("name")]
    public string? Name { get; set; }

    /// <summary>
    /// World coordinates, optional
    /// </summary>
    [JsonProperty("geometry")]
    public Geometry? Geometry { get; set; }

    /// <summary>
    /// Unique identifier, optional
    /// </summary>
    [JsonProperty("place_id")]
    public string? Place_id { get; set; }

    /// <summary>
    /// Business reviews, optional
    /// </summary>
    [JsonProperty("reviews")]
    public List<Review>? Reviews { get; set; }

    /// <summary>
    /// Price level, 0-4 from least to most expensive, optional
    /// </summary>
    [JsonProperty("price_level")]
    public float? Price_level { get; set; }
    /// <summary>
    /// Contains the place's rating, from 1.0 to 5.0, based on aggregated user reviews, optional
    /// </summary>
    [JsonProperty("rating")]
    public float? Rating { get; set; }

    /// <summary>
    /// Opening hours, optional, might be wrong?
    /// </summary>
    [JsonProperty("opening_hours")]
    public OpeningHours? OpeningHours { get; set; }

    /// <summary>
    /// Takeout available
    /// </summary>
    [JsonProperty("takeout")]
    public bool? Takeout { get; set; }

    public ARGeospatialAnchor? _geoAnchor;

    public bool _anchorInstantiated = false;
}
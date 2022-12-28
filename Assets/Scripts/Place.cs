using Newtonsoft.Json;
using System;
using System.Collections.Generic;
public class Place
{
    /// <summary>
    /// Business name. eg: "Sergio Tapia, John Cosack, Lucy McMillan", optional
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; }

    /// <summary>
    /// World coordinates, optional
    /// </summary>
    [JsonProperty("geometry")]
    public string Geometry { get; }

    /// <summary>
    /// Unique identifier, optional
    /// </summary>
    [JsonProperty("place_id")]
    public string Place_id { get; }

    /// <summary>
    /// Business reviews, optional
    /// </summary>
    [JsonProperty("reviews")]
    public List<Review> Reviews { get; }

    /// <summary>
    /// Price level, 0-4 from least to most expensive, optional
    /// </summary>
    [JsonProperty("price_level")]
    public float Price_level { get; }
    /// <summary>
    /// Contains the place's rating, from 1.0 to 5.0, based on aggregated user reviews, optional
    /// </summary>
    [JsonProperty("rating")]
    public float Rating { get; }

    /// <summary>
    /// Opening hours, optional, might be wrong?
    /// </summary>
    [JsonProperty("rating")]
    public DateTime OpeningHours { get; }

    public Vector3 position;


}
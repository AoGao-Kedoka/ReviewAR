using Newtonsoft.Json;
using System.Collections.Generic;
[System.Serializable]
public class Review
{

    /// <summary>
    /// Author of review
    /// </summary>
    [JsonProperty("author_name")]
    public string Name { get; set; }

    /// <summary>
    /// Rating for this review, optional
    /// </summary>
    [JsonProperty("rating")]
    public float Rating { get; set; }

    /// <summary>
    /// Review text, optional
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }
}
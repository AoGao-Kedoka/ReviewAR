using Newtonsoft.Json;
using System.Collections.Generic;

public class Review
{

    /// <summary>
    /// Author of review, optional
    /// </summary>
    [JsonProperty("author_name")]
    public string Name { get; set; }

    /// <summary>
    /// Rating for this review, optional
    /// </summary>
    [JsonProperty("rating")]
    public string Rating { get; set; }

    /// <summary>
    /// Review text, optional
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; set; }
}
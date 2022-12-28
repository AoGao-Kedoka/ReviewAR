using Newtonsoft.Json;
using System.Collections.Generic;

public class Review
{

    /// <summary>
    /// Author of review, optional
    /// </summary>
    [JsonProperty("author_name")]
    public string Name { get; }

    /// <summary>
    /// Rating for this review, optional
    /// </summary>
    [JsonProperty("rating")]
    public string Rating { get; }

    /// <summary>
    /// Review text, optional
    /// </summary>
    [JsonProperty("text")]
    public string Text { get; }
}
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace JokeGenerator.Feeds
{
    //TODO: Summary
    public class RandomJokeResponse
    {
        [JsonProperty("categories")]
        public List<string> Categories { get; set; }

        [JsonProperty("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [JsonProperty("icon_url")]
        public Uri IconUrl { get; set; }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; }

        [JsonProperty("value")]
        public string JokeText { get; set; }
    }
}
using Newtonsoft.Json;

namespace JokeCompany.ExternalFeeds.RandomName
{
    /// <summary>
    /// DTO for responses from the random name API.
    /// </summary>
    public class NameResponse
    {
        [JsonProperty("name")]
        public string FirstName { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("gender")]
        public string Gender { get; set; } // Would be nice as enum, but we don't need it.

        [JsonProperty("region")]
        public string Region { get; set; }
    }
}
using Newtonsoft.Json;

//TODO
namespace JokeGenerator.Feeds
{
    //TODO: Summary
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

        public string FullName => $"{FirstName ?? string.Empty} {Surname ?? string.Empty}";
    }
}
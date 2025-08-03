using System.Text.Json.Serialization;

namespace JokesApi.Models
{
    public class JokeResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("joke")]
        public string Joke { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}

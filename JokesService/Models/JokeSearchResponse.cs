using System.Text.Json.Serialization;

namespace JokesApi.Models
{
    public class SearchJokeResponse
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("limit")]
        public int Limit { get; set; }

        [JsonPropertyName("next_page")]
        public int NextPage { get; set; }

        [JsonPropertyName("previous_page")]
        public int PreviousPage { get; set; }

        [JsonPropertyName("results")]
        public List<JokeResult> Results { get; set; }

        [JsonPropertyName("search_term")]
        public string SearchTerm { get; set; } 

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("total_jokes")]
        public int TotalJokes { get; set; } 

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }
    }

    public class JokeResult
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("joke")]
        public string Joke { get; set; }
    }
}

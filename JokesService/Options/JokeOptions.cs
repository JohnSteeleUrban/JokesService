using System.ComponentModel.DataAnnotations;

namespace JokesApi.Options
{

    public class JokeOptions
    {
        [Required]
        public string BaseUrl { get; set; } = "https://icanhazdadjoke.com";

        [Required]
        public int SearchResultsLimit { get; set; } = 30;

        [Required]
        [Range(1, 30)]
        public int PageSize { get; set; } = 30;


    }
}

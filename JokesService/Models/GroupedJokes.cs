namespace JokesApi.Models
{
    public class GroupedJokes
    {
        public List<string> Short { get; set; } = new();
        public List<string> Medium { get; set; } = new();
        public List<string> Long { get; set; } = new();
    }
}

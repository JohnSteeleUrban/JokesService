using System.Text.RegularExpressions;
using JokesApi.Clients;
using JokesApi.Models;

namespace JokesApi.Services
{
    public class JokeService : IJokeService
    {
        private readonly IDadJokeClient _jokeClient;
        private readonly ILogger<JokeService> _logger;

        public JokeService(IDadJokeClient jokeClient, ILogger<JokeService> logger)
        {
            _jokeClient = jokeClient;
            _logger = logger;
        }
        public async Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var jokeRes = await _jokeClient.GetRandomJokeAsync(cancellationToken);
                if (jokeRes == null)
                {
                    _logger.LogError("Failed to fetch random joke. Joke response is null.");
                    return null;
                }

                return jokeRes.Joke;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while fetching random joke.");
                throw;
            }
        }

        public async Task<GroupedJokes> SearchJokesAsync(string? term, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
               var searchRes = await _jokeClient.SearchJokesAsync(term, cancellationToken);

               var groupedJokes = new GroupedJokes();

               foreach (var jokeResult in searchRes)
               {
                   var wordCount = jokeResult.Joke.Split(' ').Length;
                   var joke = EmphasizeTerm(jokeResult.Joke, term);

                   if (wordCount == 0) continue;

                    //was thinking a using a switch expression here because they look elegant, but they must return a value
                    switch (wordCount)
                    {
                        case < 10:
                            groupedJokes.Short.Add(joke);
                            break;
                        case < 20:
                            groupedJokes.Medium.Add(joke);
                            break;
                        default:
                            groupedJokes.Long.Add(joke);
                            break;
                    }

               }

               _logger.LogInformation("Successfully grouped {count} jokes by word count.", searchRes.Count);
                return groupedJokes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while searching jokes with term: {Term}", term);
                throw;
            }
        }

        private string EmphasizeTerm(string joke, string? term)
        {
            if (string.IsNullOrEmpty(term))
            {
                return joke;
            }

            //make sure special chars are escaped
            var escapedTerm = Regex.Escape(term);
            // i was originally going go just emphasize the actual word (pattern var below), but i noticed that 
            // the dad joke api will actually match the pattern within the words so i will do the same
            //var pattern = $@"\b{escapedTerm}\b";
            var res = Regex.Replace(joke, escapedTerm, $"<{term}>", RegexOptions.IgnoreCase);


            return res;
        }
    }
}

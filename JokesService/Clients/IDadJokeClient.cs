using JokesApi.Models;

namespace JokesApi.Clients
{
    public interface IDadJokeClient
    {
        Task<JokeResponse?> GetRandomJokeAsync(CancellationToken cancellationToken);
        Task<List<JokeResult>> SearchJokesAsync(string? term, CancellationToken cancellationToken);
    }
}

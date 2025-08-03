
using JokesApi.Models;

namespace JokesApi.Services
{
    public interface IJokeService
    {
        Task<string?> GetRandomJokeAsync(CancellationToken cancellationToken = default);
        Task<GroupedJokes> SearchJokesAsync(string? term, CancellationToken cancellationToken = default);
    }

}

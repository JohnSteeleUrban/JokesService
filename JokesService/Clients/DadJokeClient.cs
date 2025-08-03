using System.Net;
using System.Text.Json;
using JokesApi.Helpers;
using JokesApi.Models;
using JokesApi.Options;
using Microsoft.Extensions.Options;

namespace JokesApi.Clients
{
    public class DadJokeClient : IDadJokeClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<DadJokeClient> _logger;
        private readonly JokeOptions _options;

        public DadJokeClient(HttpClient httpClient, IOptions<JokeOptions> options, ILogger<DadJokeClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<JokeResponse?> GetRandomJokeAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                var response = await _httpClient.GetAsync("/", cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch random joke. Status code: {StatusCode}, Content: {Content}", response.StatusCode, content);
                    return null;
                }
                return Utilities.TryDeserialize<JokeResponse>(content, _logger);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Exception thrown while fetching random joke.");
                throw;
            }

        }


        public async Task<List<JokeResult>> SearchJokesAsync(string? term, CancellationToken cancellationToken)
        {
            //i like to add info  logs at the start of some methods to help with debugging if needed
            _logger.LogInformation($"Starting {nameof(SearchJokesAsync)}...");
            
            try
            {
                var page = 1;

                //defaulted to 30 as required by instructions - can support more and paging if needed just change the appsettings.Development.json
                var targetCount = _options.SearchResultsLimit; 
                var pageLimit = _options.PageSize;
                var jokes = new List<JokeResult>();

                while (jokes.Count < targetCount)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var url = $"/search?limit={pageLimit}&page={page}&term={WebUtility.UrlEncode(term ?? string.Empty)}";
                    var response = await _httpClient.GetAsync(url, cancellationToken);
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);

                    if (!response.IsSuccessStatusCode)
                    {
                        _logger.LogError("Failed to search jokes. Status code: {StatusCode}, Content: {Content}", response.StatusCode, content);
                        return [];
                    }

                    var searchRes = Utilities.TryDeserialize<SearchJokeResponse>(content, _logger);

                    if (searchRes == null || searchRes.Results.Count == 0)
                    {
                        _logger.LogWarning("No jokes found for term: {Term} on page {Page}", term, page);
                        break;
                    }
                    page++;

                    var missingJokeCount = targetCount - jokes.Count;
                    // pull the missing amount, if not enough will continue to next page 
                    // 'Take' will just return all if missing count is greater than results count
                    //I highlighted this because i thought it would error until i tested it
                    jokes.AddRange(searchRes.Results.Take(missingJokeCount));

                    if (searchRes.CurrentPage >= searchRes.TotalPages)
                        break;

                }

                return jokes;


            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception thrown while searching jokes with term: {Term}", term);
                throw;
            }
           
        }
    }

}

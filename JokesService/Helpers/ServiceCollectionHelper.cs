using System.Net;
using JokesApi.Clients;
using JokesApi.Options;
using JokesApi.Services;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using Polly;
using Polly.Retry;

namespace JokesApi.Helpers
{
    public static class ServiceCollectionHelper
    {
        public static IServiceCollection AddDadJokeServices(this IServiceCollection services)
        {
            services.AddScoped<IJokeService, JokeService>();

            services.AddHttpClient<IDadJokeClient, DadJokeClient>()
                .ConfigureHttpClient((serviceProvider, httpClient) =>
                {
                    var jokeOptions = serviceProvider.GetRequiredService<IOptions<JokeOptions>>().Value;
                    httpClient.BaseAddress = new Uri(jokeOptions.BaseUrl);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                })
                // I like to use polly for retries.  Its easy to use and is cleanly added into the startup middleware pipeline.
                .AddResilienceHandler("DadJokeClientPolicy", builder =>
                {
                    builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
                    {
                        MaxRetryAttempts = 3,
                        Delay = TimeSpan.FromSeconds(2),
                        BackoffType = DelayBackoffType.Exponential,
                        ShouldHandle = new PredicateBuilder<HttpResponseMessage>()

                            // only retry on transient errors
                            .HandleResult(r =>
                                r.StatusCode == HttpStatusCode.BadGateway ||
                                r.StatusCode == HttpStatusCode.ServiceUnavailable ||
                                r.StatusCode == HttpStatusCode.GatewayTimeout ||
                                r.StatusCode == HttpStatusCode.InternalServerError)
                    });
                });
            
            return services;
        }
    }
}

using System.Text.Json;

namespace JokesApi.Helpers
{
    public static class Utilities
    {
        public static T? TryDeserialize<T>(string json, ILogger? logger = null)
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json);
            }
            catch (JsonException ex)
            {
                logger?.LogError(ex, "Failed to deserialize JSON to type {Type}. JSON: {Json}", typeof(T).Name, json);
                return default;
            }
        }

    }
}

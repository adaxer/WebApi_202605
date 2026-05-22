using MovieBase.Common;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace MovieBase.ClientLib;

public class MoviesClient
{
    private HttpClient _client;

    public MoviesClient(HttpClient client)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        _client = client;
    }

    public async Task<List<Movie>> GetMovies()
    {
        var result = await _client.GetFromJsonAsync<List<Movie>>("list");
        return result ?? new List<Movie>();
    }
}

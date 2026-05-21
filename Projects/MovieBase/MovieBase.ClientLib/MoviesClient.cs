using MovieBase.Common;
using System.Net.Http.Json;
using System.Reflection.Metadata;

namespace MovieBase.ClientLib;

public class MoviesClient
{
    const string BaseUrl = "https://localhost:7184/movies/";
    private HttpClient client;

    public MoviesClient()
    {
        client = new HttpClient();
        client.BaseAddress = new Uri(BaseUrl);
    }

    public async Task<List<Movie>> GetMovies()
    {
        var result = await client.GetFromJsonAsync<List<Movie>>("list");
        return result ?? new List<Movie>();
    }
}

using Microsoft.Extensions.DependencyInjection;
using MovieBase.ClientLib;

var serviceCollection = new ServiceCollection();
serviceCollection.AddHttpClient<MoviesClient>(client => client.BaseAddress = new Uri("https://localhost:7184/movies/", UriKind.Absolute));

var services = serviceCollection.BuildServiceProvider();

//var client = new GeneratedClient("https://localhost:7184", new HttpClient());

Console.ReadLine();

//var httpClient = new HttpClient { BaseAddress = new Uri("https://localhost:7184/movies/", UriKind.Absolute) };
//var betterClient = new MoviesClient(httpClient);

var betterClient = services.GetRequiredService<MoviesClient>();

var movies = await betterClient.GetMovies();

foreach (var movie in movies)
{
    Console.WriteLine(movie.Title);
}

Console.ReadLine();

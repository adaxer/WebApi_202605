using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MovieBase.Common.Data;
using System.Diagnostics;

namespace MovieBase.Common;

public class MovieService
{
    private readonly MoviesContext context;
    private readonly ILogger<MovieService> logger;

    // In "sauberer" Architektur noch eine Abstraktion => Repository-Pattern
    public MovieService(MoviesContext context, ILogger<MovieService> logger)
    {
        this.context = context;
        this.logger = logger;
    }

    public ValueTask<Movie?> FindMovie(int id)
    {
        return context.Movies.FindAsync(id);
    }

    public Task<List<Movie>> GetMovies()
    {
        return context.Movies.ToListAsync();
    }

    // Ev. mit Resultobject
    public async Task<Movie?> SaveMovie(Movie movie)
    {
        try
        {
            if (movie.Id <= 0)
            {
                await context.Movies.AddAsync(movie);
            }
            else
            {
                var savedMovie = await context.Movies.FindAsync(movie.Id);
                context.Entry(savedMovie!).CurrentValues.SetValues(movie);
            }
            await context.SaveChangesAsync();
            return movie;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Saving failed");
            return null;
        }
    }
}

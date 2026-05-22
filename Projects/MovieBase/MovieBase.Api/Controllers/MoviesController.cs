using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using MovieBase.Common;
using System.Net.Mime;
using System.Reflection;

namespace MovieBase.Api.Controllers;

[ApiController]
[Route("[controller]/[action]")]
[Produces(MediaTypeNames.Application.Json, MediaTypeNames.Application.Xml)]
public class MoviesController : ControllerBase
{
    private readonly ILogger<MoviesController> _logger;
    private readonly MovieService _movieService;

    public MoviesController(ILogger<MoviesController> logger, MovieService movieService)
    {
        _logger = logger;
        _movieService = movieService;
    }

    [HttpGet(Name = "GetMovies")]
    public async Task<IActionResult> List()
    {
        var result = await _movieService.GetMovies();
        return Ok(result);
    }

    //public async void ButtonClick(object sender, EventArgs e)
    //{
    //    Products = await DataService.GetProducts();
    //    List.Items = Products;
    //}

    //public  IActionResult List()
    //{
    //    var result = _movieService.GetMovies();
    //    return Ok(result);
    //}

    [HttpGet("{id}", Name = "Details")]
    [Authorize(Policy ="BasicAuth")]
    public async Task<IActionResult> Details(int id)
    {
        var result = await _movieService.FindMovie(id);
        return (result == null)
            ? NotFound()
            : Ok(result);
    }

    [HttpPost(Name = "CreateMovie")]
    public async Task<IActionResult> Create([FromBody] Movie newMovie)
    {
        var movie = await _movieService.SaveMovie(newMovie);
        if (movie != null)
        {
            return CreatedAtAction("Details", new { id = movie.Id }, movie);
        }
        return BadRequest();
    }

    [HttpPut(Name ="Update")]
    public async Task<IActionResult> Update([FromBody] Movie movie)
    {
        var updatedMovie = await _movieService.SaveMovie(movie);
        if (updatedMovie != null)
        {
            return Ok(updatedMovie);
        }
        return BadRequest();
    }


    [HttpPatch("{id}")]
    public async Task<IActionResult> Patch(int id, [FromBody] JsonPatchDocument<Movie> patchDoc)
    {
        try
        {
            var movie = await _movieService.FindMovie(id);
            patchDoc.ApplyTo(movie!, ModelState);
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _movieService.SaveMovie(movie!);
            return new ObjectResult(movie);
        }
        catch (Exception ex)
        {
            return BadRequest(ex);
        }
    }

}

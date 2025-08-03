using JokesApi.Helpers;
using JokesApi.Models;
using JokesApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace JokesApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class JokesController : ControllerBase
    {
        private readonly ILogger<JokesController> _logger;
        private readonly IJokeService _jokeService;

        public JokesController(IJokeService jokeService, ILogger<JokesController> logger)
        {
            _jokeService = jokeService;
            _logger = logger;
        }

        [HttpGet("random")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<string>> GetRandomJoke(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Starting {nameof(GetRandomJoke)} request...");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var joke = await _jokeService.GetRandomJokeAsync(cancellationToken);
                if (string.IsNullOrEmpty(joke))
                {
                    return NotFound("No joke found. Something went wrong.");
                }
                return Ok(joke);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching random joke");
                return StatusCode((int)HttpStatusCode.InternalServerError, Constants.InternalServerError);
            }
        }

        [HttpGet("search")]
        [ProducesResponseType(typeof(GroupedJokes), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(GroupedJokes), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SearchJokes([FromQuery, RegularExpression(@"^\w+$", ErrorMessage = "Term must be a single word without spaces or special characters.")] string? term, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation($"Starting {nameof(SearchJokes)} request...");

            try
            {
                cancellationToken.ThrowIfCancellationRequested();
               
                var response = await _jokeService.SearchJokesAsync(term?.Trim(), cancellationToken);
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching jokes");
                return StatusCode((int)HttpStatusCode.InternalServerError, Constants.InternalServerError);
            }
        }
    }
}

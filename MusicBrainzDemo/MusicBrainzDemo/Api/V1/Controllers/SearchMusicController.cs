using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MusicBrainzDemo.Api.V1.Models;
using MusicBrainzDemo.ApplicationServices;

namespace MusicBrainzDemo.Api.V1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchMusicController : ControllerBase
    {
        private const string RecordsNotFoundErrorMessage = "No records found for the search query.";
        private const string MusicSearchExceptionMessage = "Error Occured searching for artist: {ArtistName}";

        private readonly ILogger<SearchMusicController> _logger;
        private readonly IMediator _mediator;

        public SearchMusicController(ILogger<SearchMusicController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery]SearchMusicRequest searchMusicRequest)
        {
            try
            {
                var artistsSearchResponse = await _mediator.Send(new GetArtistsQuery(searchMusicRequest.ArtistName)).ConfigureAwait(false);

                if (!artistsSearchResponse.Artists.Any())
                {
                    return NotFound(RecordsNotFoundErrorMessage);
                }

                return Ok(artistsSearchResponse.Artists);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, MusicSearchExceptionMessage, searchMusicRequest.ArtistName);
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}

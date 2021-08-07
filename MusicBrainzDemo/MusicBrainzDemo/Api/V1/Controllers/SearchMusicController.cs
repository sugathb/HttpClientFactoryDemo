using Microsoft.AspNetCore.Mvc;
using MusicBrainzDemo.Api.V1.Models;

namespace MusicBrainzDemo.Api.V1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchMusicController : ControllerBase
    {

        [HttpGet]
        public string Get([FromQuery]SearchMusicRequest searchMusicRequest)
        {
            return "Hello";
        }
    }
}

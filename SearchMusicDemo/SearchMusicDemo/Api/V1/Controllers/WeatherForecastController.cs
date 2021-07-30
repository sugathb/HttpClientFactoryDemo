using Microsoft.AspNetCore.Mvc;

namespace SearchMusicDemo.Api.V1.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class SearchMusicController : ControllerBase
    {

        [HttpGet]
        public string Get()
        {
            return "Hello";
        }
    }
}

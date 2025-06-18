using Microsoft.AspNetCore.Mvc;

namespace Maraudr.Planning.Endpoints.Controllers
{
    [ApiController]
    [Route("api/planning")]
    public class PlanningController : ControllerBase
    {


        private readonly ILogger<PlanningController> _logger;

        [HttpPost()]
        public Task<IResult> CreateAnEvent([FromBody] CreateEventDto request, [FromServices] ICreateEventHandler handler,
        [FromServices] IValidator<CreateEventDto> validator)
        {

        }
    }
}

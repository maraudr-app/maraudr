using FluentValidation;
using Maraudr.Planning.Application.DTOs;
using Maraudr.Planning.Application.UseCases;
using Maraudr.Planning.Endpoints.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Maraudr.Planning.Endpoints.Controllers
{
    [ApiController]
    [Route("api/planning")]
    public class PlanningController : ControllerBase
    {
        [HttpPost()]
        [Authorize]
        public async Task<IResult>  CreateAnEvent([FromBody] CreateEventDto request, [FromServices] ICreateAnEventHandler handler,
        [FromServices] IValidator<CreateEventDto> validator)
        {
            try
            {
                var userId = User.GetUserId();
                var eventId = await handler.HandleAsync(userId, request);
                return Results.Ok(eventId);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
            
        }
    }
}

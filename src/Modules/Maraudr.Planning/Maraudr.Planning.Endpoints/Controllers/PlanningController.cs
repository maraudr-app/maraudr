using System.Security.Claims;
using FluentValidation;
using Maraudr.Planning.Application.DTOs;
using Maraudr.Planning.Application.UseCases;
using Maraudr.Planning.Domain.ValueObjects;
using Maraudr.Planning.Endpoints.Identity;
using Microsoft.AspNetCore.Authorization;
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
                return Results.Ok(new { EventId = eventId, AssociationId = request.AssociationId });
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
            
        }

        [HttpDelete("{id:guid}")]
        [Authorize]
        public async Task<IResult> DeleteAnEvent(Guid id,[FromServices] IDeleteAnEventHandler handler)
        {
            var userId = User.GetUserId();
            var role = User.GetUserRoleEnum();
            try
            {
                await handler.HandleAsync(userId,role, id);
                return Results.Ok();

            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpGet("all-events/{associationId:guid}")]
        [Authorize]
        public async Task<IResult> GetAllAssociationEvents(Guid associationId,[FromServices] IGetAllAssociationEventsHandler handler)
        {
            var userId = User.GetUserId();
            try
            {
                var events = await handler.HandleAsync(associationId,userId);
                return Results.Ok(events);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpGet("my-events")]
        [Authorize]
        public async Task<IResult> GetAllEventsOfUser([FromServices] IGetAllEventsOfUserHandler handler)
        {
            var userId = User.GetUserId();
            try
            {
                var events =await handler.HandleAsync(userId);
                return Results.Ok(events);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpGet("my-events/{associationId:guid}")]
        [Authorize]
        public async Task<IResult> GetAllEventsOfUserInAssociation(Guid associationId ,[FromServices] IGetAllEventsOfUserInAssociationHandler handler)
        {
            var userId = User.GetUserId();
            try
            {
                var events  = await handler.HandleAsync(userId,associationId);
                return Results.Ok(events);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpGet("events/{eventId:guid}")]
        [Authorize]
        public async Task<IResult> GetEventById(Guid eventId ,[FromServices] IGetAnEventByIdHandler handler)
        {
            try
            {
                var events  = await handler.HandleAsync(eventId);
                return Results.Ok(events);
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpPost("create-planning")]
        public async Task<IResult> CreatePlanning(
            [FromBody] CreatePlanningRequest request, 
            [FromServices] ICreatePlanningHandler handler,
            [FromServices] IValidator<CreateEventDto> validator,
            [FromHeader(Name = "X-Api-Key")] string apiKey)
        {
            if (apiKey != Environment.GetEnvironmentVariable("ASSOCIATION_API_KEY"))
            {
                return Results.Unauthorized();
            }
    
            try
            {
                if (request.AssociationId == Guid.Empty)
                {
                    return Results.BadRequest("associationId is required");
                }
                var id = await handler.HandleAsync(request.AssociationId);
                return Results.Created($"/create-planning/{id}", new { Id = id });
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        
        [HttpPost("start-event/{id}")]
        [Authorize]
        public async Task<IResult> StartEvent(Guid id,
            [FromServices] IChangeEventStatusHandler handler)
        {
            try
            {
                var userId = User.GetUserId();
                await handler.HandleAsync(userId,id,Status.ONGOING);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpPost("cancel-event/{id}")]
        [Authorize]

        public async Task<IResult> CancelEvent(Guid id,
            [FromServices] IChangeEventStatusHandler handler)
        {
            try
            {
                var userId = User.GetUserId();
                await handler.HandleAsync(userId,id,Status.CANCELED);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        [HttpPost("finish-event/{id}")]
        [Authorize]
        public async Task<IResult> FinishEvent(Guid id,
            [FromServices] IChangeEventStatusHandler handler)
        {
            try
            {
                var userId = User.GetUserId();
                await handler.HandleAsync(userId,id,Status.FINISHED);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }
        
        
        [HttpPatch("update-event/{eventId}")]
        [Authorize]
        public async Task<IResult> UpdateEvent(Guid eventId,[FromBody]UpdateEventRequest request,
            [FromServices] IUpdateEventHandler handler)
        {
            try
            {
                var userId = User.GetUserId();
                await handler.HandleAsync(userId,eventId,request);
                return Results.Ok();
            }
            catch (Exception e)
            {
                return Results.BadRequest(e.Message);
            }
        }


        
        
    }
}

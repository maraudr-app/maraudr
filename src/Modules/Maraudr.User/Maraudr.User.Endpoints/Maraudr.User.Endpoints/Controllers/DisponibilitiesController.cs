using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using Application.UseCases.Disponibilities.CreateDisponibility;
using Application.UseCases.Disponibilities.DeleteDisponiblity;
using Application.UseCases.Disponibilities.GetAllAssociationUsersDipsonibilities;
using Application.UseCases.Disponibilities.GetUsersDipsonibilities;
using Application.UseCases.Disponibilities.GetUsersFutureDisponibilities;
using Application.UseCases.Disponibilities.UpdateDisponibility;
using FluentValidation;
using Maraudr.User.Domain.ValueObjects.Users;
using Maraudr.User.Endpoints.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Maraudr.User.Endpoints;


[ApiController]
[Route("api/users/disponibilities")]
public class DisponibilitiesController:ControllerBase
{
    [HttpPost]
    [Authorize]
    public async Task<IResult> CreatedDisponibility([FromBody]CreateDisponiblityRequest request,
        [FromServices] ICreateDisponibilityHandler handler,
        [FromServices] IValidator<CreateDisponiblityRequest> validator)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.Errors);
        }
        
        var userId = User.GetUserId();

        try
        {
            await handler.HandleAsync(userId, request);
            return Results.Created();

        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }
    
    [HttpPut("{id}")]
    [Authorize]
    public async Task<IResult> UpdateDisponibility(Guid id,[FromBody]UpdateDisponiblityRequest request,
        [FromServices] IUpdateDisponibilityHandler handler
       )
    {
        var userId = User.GetUserId();

        try
        {
            await handler.HandleAsync(id,userId, request);
            return Results.Ok(id);

        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    
    [HttpGet("{associationId}")]
    [Authorize]
    public async Task<IEnumerable<Disponibility?>> GetUserDisponbilities(Guid associationId,
        [FromServices] IGetUsersDisponibilitiesHandler handler)
    {
        var userId = User.GetUserId();

        try
        {
            return await handler.HandleAsync(userId,associationId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
    
    [HttpGet("all/{associationId}")]
    [Authorize]
    public async Task<IEnumerable<Disponibility?>> GetAllAssociationUsesrDisponbilities(Guid associationId,
        [FromServices] IGetAllAssociationUsersDipsonibilitiesHandler handler)
    {
        var userId = User.GetUserId();

        try
        {
            return await handler.HandleAsync(userId,associationId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }
    
    [HttpGet("futur/{associationId}")]
    [Authorize]
    public async Task<IEnumerable<Disponibility?>> GetUserFutureDisponbilities(Guid associationId,
        [FromServices] IGetUsersFutureDipsonibilitiesHandler handler)
    {
        var userId = User.GetUserId();

        try
        {
            return await handler.HandleAsync(userId,associationId);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return [];
        }
    }

    [HttpDelete("{disponibilityId}")]
    [Authorize]
    public async Task<IResult> DeleteDisponibility(Guid disponibilityId, 
        [FromServices] IDeleteDisponibilityHandler handler
    )
    {
        var userId = User.GetUserId();

        try
        { 
            await handler.HandleAsync(userId,disponibilityId);
            return Results.Ok();

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return Results.BadRequest();
        }
    }

    //Recuperer dispo si moi même ou si membre meme equipe  ou memebre meme association
    
    //[HttpGet]
    
    
}
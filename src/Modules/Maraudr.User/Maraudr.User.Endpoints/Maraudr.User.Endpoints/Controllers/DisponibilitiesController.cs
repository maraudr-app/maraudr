using Application.DTOs.DisponibilitiesQueriesDtos.Requests;
using Application.UseCases.Disponibilities.CreateDisponibility;
using Application.UseCases.Disponibilities.UpdateDisponibility;
using FluentValidation;
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
            return Results.Created();

        }
        catch (Exception e)
        {
            return Results.Problem(e.Message);
        }
    }

    
    [HttpGet()]
    [Authorize]
    public async Task<IResult> GetUserDisponbilities([FromBody]CreateDisponiblityRequest request,
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
    
    //Recuperer dispo si moi même ou si membre meme equipe  ou memebre meme association
    
    //[HttpGet]
    
    
}
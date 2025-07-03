using System.Security.Claims;
using Amazon.S3;
using Maraudr.Document.Application;
using Maraudr.Document.Domain;
using Maraudr.Document.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DocumentContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentStorageService, S3DocumentStorageService>();
builder.Services.AddScoped<DocumentService>();

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAuthorization();
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["Jwt:Authority"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapPost("/api/associations/{associationId:guid}/documents", async (
    [FromForm] UploadDocumentRequest request,
    Guid associationId,
    DocumentService service,
    ClaimsPrincipal user
) =>
{
    // TODO: check if user is allowed
    await service.UploadDocumentAsync(request, associationId);
    return Results.Ok();
}).RequireAuthorization();

app.MapGet("/api/associations/{associationId:guid}/documents", async (
    Guid associationId,
    DocumentService service,
    ClaimsPrincipal user
) =>
{
    // TODO: check if user is allowed
    var docs = await service.GetDocumentsAsync(associationId);
    return Results.Ok(docs);
}).RequireAuthorization();

app.Run();
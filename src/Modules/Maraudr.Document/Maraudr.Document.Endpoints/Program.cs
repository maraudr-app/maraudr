using System.Security.Claims;
using Amazon.S3;
using Maraudr.Document.Application;
using Maraudr.Document.Domain;
using Maraudr.Document.Endpoints;
using Maraudr.Document.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DocumentContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
    options.EnableSensitiveDataLogging();
    options.LogTo(Console.WriteLine);
});

builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.SetMinimumLevel(LogLevel.Debug);
});

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentStorageService, S3DocumentStorageService>();
builder.Services.AddScoped<DocumentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient("association", client =>
{
    client.BaseAddress = new Uri("http://association:8080");
});

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAuthorization();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://maraudr.eu")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});


var app = builder.Build();
app.UseCors("AllowFrontend");

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();


app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        Console.WriteLine("🔴 UNHANDLED EXCEPTION:");
        Console.WriteLine(ex.ToString()); // stack complète
        throw;
    }
});

app.MapPost("/upload/{associationId:guid}", [Authorize, IgnoreAntiforgery] async (
    IFormFile file,
    Guid associationId,
    DocumentService service,
    HttpContext httpContext,
    IHttpClientFactory httpClientFactory
) =>
{
    if (file.Length == 0)
    {
        return Results.BadRequest("File is required.");
    }
    
    var request = new UploadDocumentRequest { File = file };
    await service.UploadDocumentAsync(request, associationId);
    return Results.Created();
});


app.MapGet("/download/{associationId:guid}", [Authorize] async (
    Guid associationId,
    DocumentService service
) =>
{
    var docs = await service.GetDocumentsAsync(associationId);
    return Results.Ok(docs);
});

app.MapDelete("/delete/{associationId:guid}/document/{documentId:guid}", [Authorize] async (
    Guid associationId,
    Guid documentId,
    DocumentService service
) =>
{
    try
    {
        await service.DeleteDocumentAsync(documentId, associationId);
        return Results.NoContent();
    }
    catch (KeyNotFoundException)
    {
        return Results.NotFound();
    }
});




app.Run();
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
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentStorageService, S3DocumentStorageService>();
builder.Services.AddScoped<DocumentService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddAuthorization();
builder.Services.AddAuthenticationServices(builder.Configuration);
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
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

app.MapPost("/document/upload/{associationId:guid}", [Authorize, IgnoreAntiforgery] async (
    IFormFile file,
    Guid associationId,
    DocumentService service
) =>
{
    var request = new UploadDocumentRequest { File = file };
    await service.UploadDocumentAsync(request, associationId);
    return Results.Ok();
});


app.MapGet("/document/download/{associationId:guid}", [Authorize] async (
    Guid associationId,
    DocumentService service
) =>
{
    var docs = await service.GetDocumentsAsync(associationId);
    return Results.Ok(docs);
});

app.MapDelete("/document/delete/{associationId:guid}/document/{documentId:guid}", [Authorize] async (
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
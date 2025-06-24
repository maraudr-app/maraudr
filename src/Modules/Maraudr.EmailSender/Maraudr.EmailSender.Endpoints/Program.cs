using Maraudr.EmailSender.Application;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail;
using Maraudr.EmailSender.Domain.Interfaces;
using Maraudr.EmailSender.Endpoints.MailSettings;
using Maraudr.EmailSender.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers(); 

builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));






var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();



app.UseAuthorization();

app.MapControllers();



app.MapPost("/email/send-welcome", async (
    [FromBody] MailToQuery query,
    ISendWelcomeEmailHandler handler) =>
{

    await handler.HandleAsync(query);

});
app.Run();

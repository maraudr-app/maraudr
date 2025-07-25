using Maraudr.EmailSender.Application;
using Maraudr.EmailSender.Application.Dtos;
using Maraudr.EmailSender.Application.UseCases;
using Maraudr.EmailSender.Application.UseCases.SendWelcomeEmail;
using Maraudr.EmailSender.Endpoints.MailSettings;
using Maraudr.EmailSender.Infrastructure;
using Microsoft.AspNetCore.Mvc;

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
//app.UseApiKeyAuth();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/email/send-welcome", async (
    [FromBody] MailToQuery query,
    ISendWelcomeEmailHandler handler) =>
{

    await handler.HandleAsync(query);

});

app.MapPost("/email/send-reset-link", async (
    [FromBody] ResetPasswordMailRequest query,
    ISendResetLinkEmailHandler handler) =>
{

    await handler.HandleAsync(query);

});


app.MapPost("/email/send-invit-link", async (
    [FromBody] SendInvitationRequest query,
    ISendInvitationMailHandler handler) =>
{

    await handler.HandleAsync(query);

});


app.MapPost("/email/send-event-notify-batch", async (
    [FromBody] SendNotificationBatchQuery query,
    ISendNotificationBatch handler) =>
{
    try
    {
        await handler.HandleAsync(query);
    }
    catch (Exception e)
    {
        Results.BadRequest(e.StackTrace+ e.Message);
    }

});

app.Run();

using System.Text;
using MCP.Maraudr.Application.Dtos;
using MCP.Maraudr.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Maraudr.MCP.Endpoints.Controllers;

[ApiController]
[Route("api/chat")]
public class ChatController(IChatService chatService) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<ChatResponseDto>> ProcessChat([FromBody] ChatRequestDto request)
    {
        var response = await chatService.ProcessChatAsync(request);
        return Ok(response);
    }

    [HttpPost("stream")]
    public async Task<IActionResult> ProcessStreamingChat([FromBody] ChatRequestDto request)
    {
        var responseStream = await chatService.ProcessStreamingChatAsync(request);

        Response.Headers.Append("Content-Type", "text/plain; charset=utf-8");
        Response.Headers.Append("Cache-Control", "no-cache");
        Response.Headers.Append("Connection", "keep-alive");

        return new ActionResult(async () =>
        {
            await foreach (var chunk in responseStream)
            {
                await Response.WriteAsync($"data: {chunk}\n\n");
                await Response.Body.FlushAsync();
            }
        });
    }
}

public class ActionResult(Func<Task> action) : IActionResult
{
    public async Task ExecuteResultAsync(ActionContext context)
    {
        await action();
    }
}
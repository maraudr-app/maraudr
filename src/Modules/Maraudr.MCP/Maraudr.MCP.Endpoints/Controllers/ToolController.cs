using MCP.Maraudr.Application.Dtos;
using MCP.Maraudr.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Maraudr.MCP.Endpoints.Controllers;

[ApiController]
[Route("api/tools")]
public class ToolsController : ControllerBase
{
    private readonly IToolService _toolService;

    public ToolsController(IToolService toolService)
    {
        _toolService = toolService;
    }

    [HttpGet]
    public async Task<ActionResult<ToolListResponseDto>> GetAvailableTools()
    {
        var tools = await _toolService.GetAvailableToolsAsync();
        return Ok(tools);
    }

    [HttpPost("call")]
    public async Task<ActionResult<ToolCallResponseDto>> CallTool([FromBody] ToolCallRequestDto request)
    {
        var result = await _toolService.CallToolAsync(request);
        return Ok(result);
    }
}
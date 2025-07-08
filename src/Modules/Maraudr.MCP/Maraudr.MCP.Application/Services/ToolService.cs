using Maraudr.MCP.Domain.Interfaces;
using MCP.Maraudr.Application.Dtos;
using MCP.Maraudr.Application.Services.Interfaces;

namespace MCP.Maraudr.Application.Services;

public class ToolService(IMCPRepository mcpRepository) : IToolService
{
    public async Task<ToolListResponseDto> GetAvailableToolsAsync()
    {
        try
        {
            var tools = await mcpRepository.GetAvailableToolsAsync();
            var toolDtos = tools.Select(t => new McpToolDto(t.Name, t.Description)).ToList();
                
            return new ToolListResponseDto(toolDtos);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("Failed to retrieve tools", ex);
        }
    }

    public async Task<ToolCallResponseDto> CallToolAsync(ToolCallRequestDto request)
    {
        try
        {
            var result = await mcpRepository.CallToolAsync(request.ToolName, request.Arguments);
                
            return new ToolCallResponseDto(
                result.Result,
                result.IsSuccess,
                result.ErrorMessage
            );
        }
        catch (Exception ex)
        {
            return new ToolCallResponseDto(
                null!,
                false,
                $"Failed to call tool: {ex.Message}"
            );
        }
    }
}
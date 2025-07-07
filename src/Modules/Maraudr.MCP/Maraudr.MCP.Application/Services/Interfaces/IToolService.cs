using MCP.Maraudr.Application.Dtos;

namespace MCP.Maraudr.Application.Services.Interfaces;


    public interface IToolService
    {
        Task<ToolListResponseDto> GetAvailableToolsAsync();
        Task<ToolCallResponseDto> CallToolAsync(ToolCallRequestDto request);
    }

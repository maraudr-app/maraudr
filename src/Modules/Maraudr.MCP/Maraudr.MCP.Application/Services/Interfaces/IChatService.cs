using MCP.Maraudr.Application.Dtos;

namespace MCP.Maraudr.Application.Services.Interfaces;
public interface IChatService
{
    Task<ChatResponseDto> ProcessChatAsync(ChatRequestDto request);
    Task<IAsyncEnumerable<string>> ProcessStreamingChatAsync(ChatRequestDto request);
}
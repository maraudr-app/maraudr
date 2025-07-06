namespace MCP.Maraudr.Application.Dtos;

    
public record ChatRequestDto(
        string Message,
        List<ChatMessageDto>? ConversationHistory = null
    );

public record ChatMessageDto(string Role, string Content);

public record ChatResponseDto(
        string Response,
        List<ChatMessageDto> ConversationHistory
    );

public record ToolListResponseDto(List<McpToolDto> Tools);
public record McpToolDto(string Name, string Description);

public record ToolCallRequestDto(
        string ToolName,
        Dictionary<string, object> Arguments
    );

public record ToolCallResponseDto(object Result, bool IsSuccess, string? ErrorMessage = null);




// public record GeoDataDto(Guid Id, 
//  Guid GeoStoreId ,
//  double Latitude ,
//  double Longitude ,
//  DateTime ObservedAt, 
//  string? Notes , GeoStore? GeoStore)
namespace MCP.Maraudr.Application.Services;

public interface IRequestContext
{
    string? CurrentUserJwt { get; set; }
}

public class RequestContext : IRequestContext
{
    public string? CurrentUserJwt 
    { 
        get => JwtAccessor.GetCurrentJwt();
        set => JwtAccessor.SetCurrentJwt(value);
    }
}
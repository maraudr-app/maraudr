
using System.Threading;

namespace MCP.Maraudr.Application.Services;

public static class JwtAccessor
{
    private static ThreadLocal<string> _currentJwt = new ThreadLocal<string>();

    public static void SetCurrentJwt(string jwt) => _currentJwt.Value = jwt;
    public static string GetCurrentJwt() => _currentJwt.Value;
}
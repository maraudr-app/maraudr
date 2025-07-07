using Microsoft.AspNetCore.Antiforgery;

namespace Maraudr.Document.Endpoints;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class IgnoreAntiforgeryAttribute : Attribute, IAntiforgeryMetadata
{
    public bool RequiresValidation { get; }
}
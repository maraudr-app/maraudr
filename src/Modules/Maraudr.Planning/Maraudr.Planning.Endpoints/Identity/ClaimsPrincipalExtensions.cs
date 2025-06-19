using System.Security.Claims;
using Maraudr.Planning.Domain.ValueObjects;

namespace Maraudr.Planning.Endpoints.Identity

{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.Parse(claim);
        }

        public static string? GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email) ??
                   principal.FindFirstValue("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress");
        }
        
        public static string GetUserRole(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Role) ??
                   principal.FindFirstValue("role") ??
                   principal.FindFirstValue("Role") ??
                   "Member"; // Valeur par défaut si aucun rôle n'est trouvé
        }
        
        public static Role GetUserRoleEnum(this ClaimsPrincipal principal)
        {
            string roleString = principal.GetUserRole();
    
            if (Enum.TryParse<Role>(roleString, true, out var role))
                return role;
    
            return Role.Member; 
        }
    }
}
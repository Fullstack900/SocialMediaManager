using System.Security.Claims;

namespace SocialMedia_Backend.Utitlities;

public static class HttpContextExtensions
{
    public static Guid GetUserId(this HttpContext httpContext)
    {
        string idClaim = httpContext.User.Claims.First(c => c.Type == "id").Value;
        Guid userId = Guid.Parse(idClaim);
        return userId;
    }

    public static string GetEmail(this HttpContext httpContext)
    {
        string email = httpContext.User.Claims.First(c => c.Type == ClaimTypes.Email).Value;
        return email;
    }
}
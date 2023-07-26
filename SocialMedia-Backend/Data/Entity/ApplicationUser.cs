using Microsoft.AspNetCore.Identity;

namespace SocialMedia_Backend.Data.Entity;

public class ApplicationUser : IdentityUser<Guid>
{
    public bool IsActive { get; set; }
}
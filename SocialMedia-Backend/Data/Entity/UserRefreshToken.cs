using Microsoft.AspNetCore.Identity;
using SocialMedia_Backend.Model.Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SocialMedia_Backend.Data.Entity;

public class UserSessionToken
{
    [Key]
    public Guid Id { get; set; }


    [ForeignKey(nameof(UserId))]
    public Guid UserId { get; set; }

    public JWTTokenType Type { get; set; }

    [Required]
    public string Token { get; set; }

    public DateTimeOffset TokenIssuedAt { get; set; }

    public DateTimeOffset TokenExpiresAt { get; set; }

    public bool IsActive { get; set; }

    public ApplicationUser User { get; set; }
}

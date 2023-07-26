using System.ComponentModel.DataAnnotations;

namespace SocialMedia_Backend.Model.DTO.Auth;

public class ForgotPasswordRequest
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}

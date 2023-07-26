using System.ComponentModel.DataAnnotations;

namespace SocialMedia_Backend.Model.DTO.Auth.Request
{
    public class RegisterationRequest
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string Email { get; set; }

        public string Password { get; set; }

        public bool? IsActive { get; set; } = false;

        public List<string?> Roles { get; set; } = new List<string?>();

    }
}

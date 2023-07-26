namespace SocialMedia_Backend.Model.DTO.Auth.Response;

public class LoginResponse
{
    public string UserName { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTimeOffset AccessTokenExpiresAt { get; set; }
    public DateTimeOffset RefreshTokenExpiresAt { get; set; }
}

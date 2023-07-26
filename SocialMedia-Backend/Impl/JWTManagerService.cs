using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SocialMedia_Backend.Data;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Error;
using SocialMedia_Backend.Model.Constant;
using SocialMedia_Backend.Model.DTO.Auth;
using SocialMedia_Backend.Model.Enum;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMedia_Backend.Impl;

public interface IJWTManagerService
{
    Task<UserSessionToken> GetToken(string email, JWTTokenType type);
    Task<string> GetTokenValue(string email, JWTTokenType type);
    Task DeleteUserSessionToken(Guid userId, JWTTokenType type);
}

public class JWTManagerService : IJWTManagerService
{
    private readonly JwtTokenConfig _configuration;
    private ApplicationDbContext _dbContext;

    public JWTManagerService(IConfiguration configuration, ApplicationDbContext dbContext)
    {
        _configuration = configuration.GetSection("jwtConfig").Get<JwtTokenConfig>();
        _dbContext = dbContext;
    }

    public async Task<UserSessionToken> GetToken(string email, JWTTokenType type)
    {
        if (string.IsNullOrEmpty(email))
        {
            throw new NullReferenceException("Email does not found.");
        }

        ApplicationUser? identity = await _dbContext.Users.Where(x => x.Email.ToLower().Equals(email.ToLower())
        || x.UserName.ToLower().Equals(email.ToLower())).FirstOrDefaultAsync();
        if (identity == null)
        {
            throw new NullReferenceException("User does not exist.");
        }

        UserSessionToken? entity;

        switch (type)
        {
            case JWTTokenType.ActivateIdentity:
                entity = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == identity.Id && x.TokenExpiresAt > DateTimeOffset.UtcNow && x.Type == JWTTokenType.ActivateIdentity && x.IsActive);
                break;
            case JWTTokenType.ForgotPassword:
                entity = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == identity.Id && x.TokenExpiresAt > DateTimeOffset.UtcNow && x.Type == JWTTokenType.ForgotPassword && x.IsActive);
                break;
            case JWTTokenType.RefreshToken:
                entity = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == identity.Id && x.TokenExpiresAt > DateTimeOffset.UtcNow && x.Type == JWTTokenType.RefreshToken && x.IsActive);
                break;
            case JWTTokenType.AccessToken:
                entity = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == identity.Id && x.TokenExpiresAt > DateTimeOffset.UtcNow && x.Type == JWTTokenType.AccessToken && x.IsActive);
                break;
            default:
                return await GenerateToken(identity, type);
        }

        if (entity != null)
        {
            return entity;
        }
        else
        {
            return await GenerateToken(identity, type);
        }
    }

    public async Task<string> GetTokenValue(string email, JWTTokenType type)
    {
        UserSessionToken userSessionToken = await GetToken(email, type);
        return userSessionToken.Token;
    }

    private async Task<UserSessionToken> GenerateToken(ApplicationUser identity, JWTTokenType tokenType)
    {
        bool isRefreshToken = tokenType == JWTTokenType.RefreshToken ? true : false;
        DateTime expireAt = isRefreshToken ? DateTime.UtcNow.AddMinutes(_configuration.RefreshTokenExpiration) :
                DateTime.UtcNow.AddMinutes(_configuration.AccessTokenExpiration);

        Claim[] regularClaims;

        switch (tokenType)
        {
            case JWTTokenType.ActivateIdentity:
                regularClaims = BuildClaims(identity, new[] { new Claim(ClaimStore.ActivateAccountTokenClaim, "true") });
                break;
            case JWTTokenType.ForgotPassword:
                regularClaims = BuildClaims(identity, new[] { new Claim(ClaimStore.ForgotPasswordTokenClaim, "true") });
                break;
            case JWTTokenType.RefreshToken:
                regularClaims = BuildClaims(identity, new[] { new Claim(ClaimStore.RefreshTokenClaim, "true") });
                break;
            case JWTTokenType.AccessToken:
                regularClaims = BuildClaims(identity);
                break;
            default:
                throw new BadOperationException("Please, specify correct token type.");
        }


        UserSessionToken Token = new UserSessionToken
        {
            Token = BuildToken(regularClaims, expireAt),
            TokenExpiresAt = expireAt,
            TokenIssuedAt = DateTime.UtcNow,
            Type = tokenType,
            UserId = identity.Id,
            IsActive = true,
        };

        await _dbContext.UserSessionToken.AddAsync(Token);
        await _dbContext.SaveChangesAsync();
        return Token;
    }

    public async Task DeleteUserSessionToken(Guid userId, JWTTokenType tokenType)
    {
        IQueryable<UserSessionToken> tokens = _dbContext.UserSessionToken.Where(x => x.UserId == userId && x.Type == tokenType
        && (!x.IsActive || x.TokenExpiresAt < DateTime.UtcNow));
        if (tokens != null && tokens.Count() > 0)
        {
            _dbContext.UserSessionToken.RemoveRange(tokens);
            _dbContext.SaveChanges();
        }
        var item = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == userId && x.Type == tokenType && x.IsActive);
        if (item != null)
        {
            item.IsActive = false;
            await _dbContext.SaveChangesAsync();
        }
    }

    private string BuildToken(Claim[] claims, DateTime ExpireDateTime)
    {
        SigningCredentials creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.Secret)), SecurityAlgorithms.HmacSha256Signature);
        var token = new JwtSecurityToken(
                issuer: _configuration.Issuer,
                audience: _configuration.Audience,
                notBefore: DateTime.UtcNow,
                claims: claims,
                expires: ExpireDateTime,
                signingCredentials: creds);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private Claim[] BuildClaims(ApplicationUser? user, Claim[]? predefinedClaims = null)
    {
        Claim[]? claims = new Claim[] { };
        if (user != null)
        {
            claims = new[]
            {
                new Claim("id", user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName)
            };
        }

        if (predefinedClaims != null)
        {
            claims = claims.Concat(predefinedClaims).ToArray();
        }
        //else
        //{
        //    IQueryable<RoleDataModel>? roles = _authDbContext.Role.Where(r => r.Identities.Any(i => i.Id == user.Id));
        //    IQueryable<ClaimDataModel>? roleClaims = roles.SelectMany(r => r.Claims).Distinct();
        //    IQueryable<Claim>? permissions = roleClaims.Select(c => new Claim(c.Name, ""));

        //    claims = claims.Concat(permissions).ToArray();
        //}

        return claims;
    }


    //public Token GenerateToken(string userName)
    //{
    //    return GenerateJWTTokens(userName);
    //}

    //public Token GenerateRefreshToken(string username)
    //{
    //    return GenerateJWTTokens(username);
    //}

    //public Token GenerateJWTTokens(string userName)
    //{
    //    try
    //    {
    //        var tokenHandler = new JwtSecurityTokenHandler();
    //        var tokenKey = Encoding.UTF8.GetBytes(jwtConfig["secret"]);
    //        var tokenDescriptor = new SecurityTokenDescriptor
    //        {
    //            Subject = new ClaimsIdentity(new Claim[]
    //          {
    //             new Claim(ClaimTypes.Name, userName)
    //          }),
    //            Expires = DateTime.Now.AddMinutes(1),
    //            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
    //        };
    //        var token = tokenHandler.CreateToken(tokenDescriptor);
    //        var refreshToken = GenerateRefreshToken();
    //        return new Token { Access_Token = tokenHandler.WriteToken(token), Refresh_Token = refreshToken };
    //    }
    //    catch (Exception ex)
    //    {
    //        return null;
    //    }
    //}



    //public string GenerateRefreshToken()
    //{
    //    var randomNumber = new byte[32];
    //    using (var rng = RandomNumberGenerator.Create())
    //    {
    //        rng.GetBytes(randomNumber);
    //        return Convert.ToBase64String(randomNumber);
    //    }
    //}

    //public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    //{
    //    var tokenKey = Encoding.UTF8.GetBytes(_configuration.Secret);

    //    var tokenValidationParameters = new TokenValidationParameters
    //    {
    //        ValidateIssuer = false,
    //        ValidateAudience = false,
    //        ValidateLifetime = false,
    //        ValidateIssuerSigningKey = true,
    //        IssuerSigningKey = new SymmetricSecurityKey(tokenKey),
    //        ClockSkew = TimeSpan.Zero
    //    };

    //    var tokenHandler = new JwtSecurityTokenHandler();
    //    var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
    //    JwtSecurityToken? jwtSecurityToken = securityToken as JwtSecurityToken;
    //    if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
    //    {
    //        throw new SecurityTokenException("Invalid token");
    //    }


    //    return principal;
    //}

    //public UserSessionToken AddUserRefreshToken(UserSessionToken user)
    //{
    //    _dbContext.UserSessionToken.Add(user);
    //    _dbContext.SaveChanges();
    //    return user;
    //}



    //public async Task<UserSessionToken?> GetSavedRefreshTokens(Guid userId, string refreshToken) =>
    //  await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == userId && x.Token == refreshToken);

}

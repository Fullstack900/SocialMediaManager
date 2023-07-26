using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Impl;
using SocialMedia_Backend.Model.Constant;
using SocialMedia_Backend.Model.DTO.Auth;
using SocialMedia_Backend.Model.DTO.Auth.Request;
using SocialMedia_Backend.Model.DTO.Auth.Response;
using SocialMedia_Backend.Model.DTO.Role;
using SocialMedia_Backend.Model.Enum;
using SocialMedia_Backend.Utitlities;

namespace SocialMedia_Backend.Controllers;

[ApiController]
public class AuthController : BaseApiController
{
    public AuthController(IRepositoryManager repository, IMapper mapper) : base(repository, mapper)
    {
    }

    #region Auth
    [AllowAnonymous]
    [HttpPost("SignUp")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterationRequest userRegistration)
    {
        var userResult = await _repository.UserAuthentication.RegisterUserAsync(userRegistration);
        if (userResult.Succeeded)
        {
            UserSessionToken TokenResult = await _repository.JWTManager.GetToken(userRegistration.Email, JWTTokenType.ActivateIdentity);
            return StatusCode(201, new { ActivationToken = TokenResult.Token });
        }
        return new BadRequestObjectResult(userResult);
    }

    [HttpPost("ActivateAccount")]
    [Authorize(Policy = PermissionStore.ActivateAccountTokenPolicy)]
    public async Task<IActionResult> ActivateAccount()
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Guid userId = HttpContext.GetUserId();
            ApplicationUser user = await _repository.UserAuthentication.ActivateAccount(userId);
            await _repository.JWTManager.DeleteUserSessionToken(user.Id, JWTTokenType.ActivateIdentity);
            UserSessionToken AccessToken = await _repository.JWTManager.GetToken(user.Email, JWTTokenType.AccessToken);
            UserSessionToken RefreshToken = await _repository.JWTManager.GetToken(user.Email, JWTTokenType.RefreshToken);
            return Ok(new LoginResponse
            {
                UserName = user.UserName,
                AccessToken = AccessToken.Token,
                RefreshToken = RefreshToken.Token,
                AccessTokenExpiresAt = AccessToken.TokenExpiresAt,
                RefreshTokenExpiresAt = RefreshToken.TokenExpiresAt
            });
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    [AllowAnonymous]
    [HttpPost("SignIn")]
    public async Task<IActionResult> SingInUser([FromBody] LoginRequest user)
    {
        if (!await _repository.UserAuthentication.SignInAsync(user))
        {
            return Unauthorized();
        }
        UserSessionToken AccessToken = await _repository.JWTManager.GetToken(user.Username, JWTTokenType.AccessToken);
        UserSessionToken RefreshToken = await _repository.JWTManager.GetToken(user.Username, JWTTokenType.RefreshToken);
        return Ok(new LoginResponse
        {
            UserName = user.Username,
            AccessToken = AccessToken.Token,
            RefreshToken = RefreshToken.Token,
            AccessTokenExpiresAt = AccessToken.TokenExpiresAt,
            RefreshTokenExpiresAt = RefreshToken.TokenExpiresAt
        });
    }

    [Authorize]
    [HttpPost("Signout")]
    public async Task<IActionResult> Signout()
    {
        Guid userId = HttpContext.GetUserId();
        await _repository.JWTManager.DeleteUserSessionToken(userId, JWTTokenType.AccessToken);
        await _repository.JWTManager.DeleteUserSessionToken(userId, JWTTokenType.RefreshToken);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("ResendActivationCode")]
    public async Task<IActionResult> ResendActivationCode([FromBody] ReSendActivationEmailRequest request) =>
        Ok(new { ActivationToken = await _repository.JWTManager.GetTokenValue(request.Login, JWTTokenType.ActivateIdentity) });

    [AllowAnonymous]
    [HttpPost("SendForgotPasswordCode")]
    public async Task<IActionResult> SendForgotPasswordCode([FromBody] ReSendActivationEmailRequest request) =>
        Ok(new { ForgotPasswordToken = await _repository.JWTManager.GetTokenValue(request.Login, JWTTokenType.ForgotPassword) });

    [HttpPost("UpdateForgotPassword")]
    [Authorize(Policy = PermissionStore.ForgotPasswordTokenPolicy)]
    public async Task<IActionResult> UpdateForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            Guid userId = HttpContext.GetUserId();
            return Ok(await _repository.UserAuthentication.SetForgotPassword(userId, forgotPasswordRequest.Login, forgotPasswordRequest.Password));
        }
        catch (Exception ex)
        {
            throw;
        }
    }
    #endregion

    #region Role
    [HttpGet("GetClaims")]
    [Authorize(Policy = PermissionStore.ManageRolePolicy)]
    public IActionResult GetClaims()
    {
        return Ok(ClaimStore.GetClaims().Select(x => new
        {
            Feature = x.Key,
            Permissions = x.Value
        }));
    }

    [HttpPost("CreateRole")]
    [Authorize(Policy = PermissionStore.ManageRolePolicy)]
    public async Task<IActionResult> CreateRole([FromBody] RoleCreateRequest roleCreateRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userResult = await _repository.UserAuthentication.AddRoleAsync(roleCreateRequest);
        if (userResult.Succeeded)
        {
            return StatusCode(201);
        }
        return new BadRequestObjectResult(userResult);
    }

    [HttpPut("UpdateRole/{key}")]
    [Authorize(Policy = PermissionStore.ManageRolePolicy)]
    public async Task<IActionResult> UpdateRole(Guid key, [FromBody] RoleCreateRequest roleCreateRequest)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        await _repository.UserAuthentication.UpdateRoleAsync(key, roleCreateRequest);
        return Ok("Role has been updated successfully");
    }

    [HttpGet("GetRole/{key}")]
    [Authorize(Policy = PermissionStore.ViewPostPolicy)]
    public async Task<RoleClaimResponse> GetRole(Guid key) => await _repository.UserAuthentication.GetRole(key);

    [HttpGet("GetRoles")]
    [Authorize(Policy = PermissionStore.ViewPostPolicy)]
    public IQueryable<object> GetRoles() => _repository.UserAuthentication.GetRoles().Select(x => new { x.Id, x.Name });

    [HttpDelete("DeleteRole/{key}")]
    [Authorize(Policy = PermissionStore.ManageRolePolicy)]
    public async Task<IdentityResult> DeleteRole(Guid key) => await _repository.UserAuthentication.DeleteRole(key);
    #endregion
}
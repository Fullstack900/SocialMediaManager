using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Backend.Data;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Error;
using SocialMedia_Backend.Model.DTO.Auth.Request;
using SocialMedia_Backend.Model.DTO.Role;
using SocialMedia_Backend.Model.Enum;
using SocialMedia_Backend.Utitlities;

namespace SocialMedia_Backend.Impl;

public interface IAuthenticationService
{
    #region Auth
    Task<IdentityResult> RegisterUserAsync(RegisterationRequest userForRegistration);
    Task<ApplicationUser> ActivateAccount(Guid userId);
    Task<bool> SignInAsync(LoginRequest loginDto);
    Task<IdentityResult> SetForgotPassword(Guid userId, string userName, string newPassword);
    #endregion

    #region Employee
    Task<IdentityResult> AddUserAsync(RegisterationRequest userRegistration);
    Task<IdentityResult> RemoveUserAsync(Guid userId);
    IQueryable<ApplicationUser> GetUsers();
    #endregion

    #region Role
    Task<IdentityResult> AddRoleAsync(RoleCreateRequest request);
    Task UpdateRoleAsync(Guid roleId, RoleCreateRequest request);
    IQueryable<IdentityRole<Guid>> GetRoles();
    Task<RoleClaimResponse> GetRole(Guid roleId);
    Task<IdentityResult> DeleteRole(Guid roleId);
    #endregion
}

internal sealed class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly IMapper _mapper;
    private ApplicationDbContext _dbContext;
    private ApplicationUser? _user;

    public AuthenticationService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IMapper mapper,
        ApplicationDbContext dbContext
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _dbContext = dbContext;
    }

    #region Auth
    public async Task<IdentityResult> RegisterUserAsync(RegisterationRequest userRegistration)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = userRegistration.Username,
            Email = userRegistration.Email,
            IsActive = userRegistration.IsActive.Value,
        };
        var result = await _userManager.CreateAsync(user, userRegistration.Password);
        return result;
    }

    public async Task<ApplicationUser> ActivateAccount(Guid userId)
    {
        UserSessionToken? activationToken = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == userId &&
        x.Type == JWTTokenType.ActivateIdentity);
        if (activationToken == null || activationToken.TokenExpiresAt < DateTimeOffset.UtcNow || !activationToken.IsActive)
        {
            throw new BadOperationException("Token is not valid or expired");
        }

        _user = await _userManager.FindByIdAsync(activationToken.UserId.ToString());
        if (_user == null)
        {
            throw new NullReferenceException("User does not exists.");
        }
        if (_user.EmailConfirmed)
        {
            throw new BadOperationException("User already activated");
        }
        _user.EmailConfirmed = true;
        _user.IsActive = true;
        await _dbContext.SaveChangesAsync();
        return _user;
    }

    public async Task<bool> SignInAsync(LoginRequest loginDto)
    {
        _user = await _userManager.FindByEmailAsync(loginDto.Username) ?? await _userManager.FindByNameAsync(loginDto.Username);
        if (_user != null)
        {
            if (!_user.EmailConfirmed || !_user.IsActive)
            {
                throw new BadOperationException("User is not Active yet.");
            }
            return await _userManager.CheckPasswordAsync(_user, loginDto.Password);
        }
        return false;
    }

    public async Task<IdentityResult> SetForgotPassword(Guid userId, string userName, string newPassword)
    {
        UserSessionToken? forgotPasswordToken = await _dbContext.UserSessionToken.FirstOrDefaultAsync(x => x.UserId == userId &&
        x.Type == JWTTokenType.ForgotPassword);
        if (forgotPasswordToken == null || forgotPasswordToken.TokenExpiresAt < DateTimeOffset.UtcNow || !forgotPasswordToken.IsActive)
        {
            throw new BadOperationException("Token is not valid or expired");
        }
        _user = await _userManager.FindByIdAsync(forgotPasswordToken.UserId.ToString());

        if (_user == null)
        {
            throw new NullReferenceException("User does not exists.");
        }
        if (!_user.EmailConfirmed)
        {
            throw new BadOperationException("User is not yet activated.");
        }
        if (!_user.UserName.ToLower().Equals(userName))
        {
            throw new BadOperationException("User is not valid.");
        }

        await _userManager.RemovePasswordAsync(_user);
        IdentityResult result = await _userManager.AddPasswordAsync(_user, newPassword);
        return result;
    }
    #endregion

    #region Role
    public async Task<IdentityResult> AddRoleAsync(RoleCreateRequest request)
    {
        IdentityRole<Guid> role = new IdentityRole<Guid>(request.RoleName);
        List<IdentityRoleClaim<Guid>> roleClaims = new List<IdentityRoleClaim<Guid>>();
        var result = await _roleManager.CreateAsync(role);

        if (result.Succeeded)
        {
            request.RolePermissions.ForEach(key =>
            {
                key.Permisssions.ForEach(value =>
                {
                    roleClaims.Add(new IdentityRoleClaim<Guid> { RoleId = role.Id, ClaimType = key.FeatureName, ClaimValue = value });
                });
            });
            await _dbContext.RoleClaims.AddRangeAsync(roleClaims);
            await _dbContext.SaveChangesAsync();
        }
        return result;
    }

    public async Task UpdateRoleAsync(Guid roleId, RoleCreateRequest request)
    {
        IdentityRole<Guid> role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NullReferenceException("Role does not exist.");
        }

        List<IdentityRoleClaim<Guid>> existingRoleClaims = await _dbContext.RoleClaims.Where(x => x.RoleId == roleId).ToListAsync();
        if (existingRoleClaims != null && existingRoleClaims.Count > 0)
        {
            _dbContext.RoleClaims.RemoveRange(existingRoleClaims);
        }

        List<IdentityRoleClaim<Guid>> roleClaims = new List<IdentityRoleClaim<Guid>>();
        request.RolePermissions.ForEach(key =>
        {
            key.Permisssions.ForEach(value =>
            {
                roleClaims.Add(new IdentityRoleClaim<Guid> { RoleId = role.Id, ClaimType = key.FeatureName, ClaimValue = value });
            });
        });
        await _dbContext.RoleClaims.AddRangeAsync(roleClaims);
        await _dbContext.SaveChangesAsync();
    }

    public IQueryable<IdentityRole<Guid>> GetRoles() => _roleManager.Roles;

    public async Task<RoleClaimResponse> GetRole(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NullReferenceException("Role does not exists.");
        }

        List<FeatureClaim> RolePermissions = (await _roleManager.GetClaimsAsync(role)).ToList().GroupBy(x => x.Type,
              (key, group) => new FeatureClaim { FeatureName = key, Permisssions = group.Select(x => x.Value).ToList() }).ToList();

        return new RoleClaimResponse
        {
            RoleName = role.Name,
            Id = role.Id,
            RolePermissions = RolePermissions
        };
    }

    public async Task<IdentityResult> DeleteRole(Guid roleId)
    {
        var role = await _roleManager.FindByIdAsync(roleId.ToString());
        if (role == null)
        {
            throw new NullReferenceException("Role does not exists.");
        }
        return await _roleManager.DeleteAsync(role);
    }
    #endregion

    #region Employee
    public async Task<IdentityResult> AddUserAsync(RegisterationRequest userRegistration)
    {
        ApplicationUser user = new ApplicationUser
        {
            UserName = userRegistration.Username,
            Email = userRegistration.Email,
            IsActive = false,
        };
        var result = await _userManager.CreateAsync(user, GenerateRandomPassword.GenerateRandomString(8));
        return result;
    }

    public async Task<IdentityResult> RemoveUserAsync(Guid userId)
    {
        _user = await _userManager.FindByIdAsync(userId.ToString());
        if (_user != null)
        {
            _user.IsActive = false;
            return await _userManager.UpdateAsync(_user);
        }
        throw new NullReferenceException("User is not found.");
    }

    public IQueryable<ApplicationUser> GetUsers() => _userManager.Users;

    #endregion
}
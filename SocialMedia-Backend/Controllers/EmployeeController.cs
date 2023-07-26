using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Impl;
using SocialMedia_Backend.Model.Constant;
using SocialMedia_Backend.Model.DTO;
using SocialMedia_Backend.Model.DTO.Auth.Request;
using SocialMedia_Backend.Model.Enum;

namespace SocialMedia_Backend.Controllers;
[ApiController]

public class EmployeeController : BaseApiController
{
    public EmployeeController(IRepositoryManager repository, IMapper mapper) : base(repository, mapper)
    {
    }

    [HttpPost("CreateEmployee")]
    [Authorize(Policy = PermissionStore.ManageEmployeePolicy)]
    public async Task<IActionResult> AddEmployee([FromBody] RegisterationRequest userRegistration)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var userResult = await _repository.UserAuthentication.AddUserAsync(userRegistration);
        if (userResult.Succeeded)
        {
            UserSessionToken TokenResult = await _repository.JWTManager.GetToken(userRegistration.Email, JWTTokenType.ActivateIdentity);
            return StatusCode(201, new { ActivationToken = TokenResult.Token });
        }
        return new BadRequestObjectResult(userResult);
    }

    [Authorize(Policy = PermissionStore.ManageEmployeePolicy)]
    [HttpDelete("DeleteEmployee")]
    public async Task<IActionResult> DeleteEmployee([FromBody] DeleteResourceRequest request) =>
        Ok(await _repository.UserAuthentication.RemoveUserAsync(request.Id));

    [Authorize(Policy = PermissionStore.ViewEmployeePolicy)]
    [HttpGet("GetEmployees")]
    public IQueryable<ApplicationUser> DeleteEmployee() => _repository.UserAuthentication.GetUsers();


}

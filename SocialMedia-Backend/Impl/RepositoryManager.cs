using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SocialMedia_Backend.Data;
using SocialMedia_Backend.Data.Entity;

namespace SocialMedia_Backend.Impl;

public interface IRepositoryManager
{
    IAuthenticationService UserAuthentication { get; }
    IJWTManagerService JWTManager { get; }
    Task SaveAsync();
}

public class RepositoryManager : IRepositoryManager
{
    private ApplicationDbContext _dbContext;
    private UserManager<ApplicationUser> _userManager;
    private RoleManager<IdentityRole<Guid>> _roleManager;
    private IMapper _mapper;
    private IConfiguration _configuration;

    private IAuthenticationService? _authenticationService;
    private IJWTManagerService? _jWTManagerService;

    public RepositoryManager(
        ApplicationDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        IMapper mapper,
        IConfiguration configuration
        )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _mapper = mapper;
        _configuration = configuration;
    }

    public IAuthenticationService UserAuthentication
    {
        get
        {
            if (_authenticationService is null)
                _authenticationService = new AuthenticationService(_userManager, _roleManager, _mapper, _dbContext);
            return _authenticationService;
        }
    }

    public IJWTManagerService JWTManager
    {
        get
        {
            if (_jWTManagerService is null)
                _jWTManagerService = new JWTManagerService(_configuration, _dbContext);
            return _jWTManagerService;
        }
    }

    public Task SaveAsync() => _dbContext.SaveChangesAsync();
}

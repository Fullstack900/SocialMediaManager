using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SocialMedia_Backend.Data;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Impl;
using SocialMedia_Backend.Impl.Mapping;
using SocialMedia_Backend.Model.Constant;
using System.Text;

namespace SocialMedia_Backend
{
    public static class ServiceExtensions
    {
        //public static void ConfigureLoggerService(this IServiceCollection services) =>
        //    services.AddScoped<ILogger, LoggerManager>();

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
            services.AddDbContext<ApplicationDbContext>(
                opts => opts.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        public static void ConfigureRepositoryManager(this IServiceCollection services)
            => services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureMapping(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });
            var mapperConfig = new MapperConfiguration(map =>
            {
                map.AddProfile<UserMappingProfile>();
            });
            services.AddSingleton(mapperConfig.CreateMapper());
        }

        public static void ConfigureControllers(this IServiceCollection services)
        {
            services.AddControllers(config =>
            {
                config.CacheProfiles.Add("30SecondsCaching", new CacheProfile
                {
                    Duration = 30
                });
            });
        }

        public static void ConfigureResponseCaching(this IServiceCollection services) => services.AddResponseCaching();

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(o =>
            {
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = configuration.GetSection("jwtConfig");
            var secretKey = jwtConfig["secret"];
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtConfig["issuer"],
                    ValidAudience = jwtConfig["audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
            });
        }

        public static void RegisterPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                #region AuthModule
                options.AddPolicy(PermissionStore.ActivateAccountTokenPolicy, policy => policy.RequireClaim(ClaimStore.ActivateAccountTokenClaim));
                options.AddPolicy(PermissionStore.ForgotPasswordTokenPolicy, policy => policy.RequireClaim(ClaimStore.ForgotPasswordTokenClaim));
                options.AddPolicy(PermissionStore.RefreshTokenPolicy, policy => policy.RequireClaim(ClaimStore.RefreshTokenClaim));
                #endregion

                #region RoleManagement
                options.AddPolicy(PermissionStore.ManageRolePolicy, policy => policy.RequireClaim(ClaimStore.ManageRoleClaim));
                options.AddPolicy(PermissionStore.ViewRolePolicy, policy => policy.RequireClaim(ClaimStore.ViewRoleClaim));

                #endregion

                #region PostManagement
                options.AddPolicy(PermissionStore.ManagePostPolicy, policy => policy.RequireClaim(ClaimStore.ManagePostClaim));
                options.AddPolicy(PermissionStore.ViewPostPolicy, policy => policy.RequireClaim(ClaimStore.ViewPostClaim));
                #endregion

                #region EmployeeManagement
                options.AddPolicy(PermissionStore.ManageEmployeePolicy, policy => policy.RequireClaim(ClaimStore.ManageEmployeeClaim));
                options.AddPolicy(PermissionStore.ViewEmployeePolicy, policy => policy.RequireClaim(ClaimStore.ViewEmployeeClaim));
                #endregion
            });
        }

        public static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Socail Media Manager API",
                    Version = "v1",
                    Description = "Socail Media Manager API Services.",
                    Contact = new OpenApiContact
                    {
                        Name = "Farrukh Maqsood."
                    },
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });
        }


        //public static void RegisterDependencies(this IServiceCollection services)
        //{
        //    services.AddScoped<ValidationFilterAttribute>();
        //    services.AddScoped<ValidateTeacherExists>();
        //    services.AddScoped<ValidateStudentExistsForTeacher>();
        //}
    }
}

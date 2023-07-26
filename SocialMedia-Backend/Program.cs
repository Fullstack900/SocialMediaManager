using Microsoft.EntityFrameworkCore;
using SocialMedia_Backend.Data;
using System;

namespace SocialMedia_Backend
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.ConfigureResponseCaching();

            // Add services to the container.
            //builder.Services.RegisterDependencies();
            builder.Services.ConfigureMapping();
            //builder.Services.ConfigureLoggerService();

            builder.Services.ConfigureSqlContext(builder.Configuration);
            builder.Services.ConfigureRepositoryManager();

            builder.Services.AddAuthentication();
            builder.Services.ConfigureIdentity();
            builder.Services.ConfigureJWT(builder.Configuration);
            builder.Services.RegisterPolicies();

            builder.Services.ConfigureControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.ConfigureSwagger();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseResponseCaching();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            using var scope = app.Services.CreateScope();
            await using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            await dbContext.Database.MigrateAsync();

            app.Run();
        }
    }
}
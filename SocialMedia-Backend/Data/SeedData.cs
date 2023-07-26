using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Backend.Data.Entity;
using SocialMedia_Backend.Model.Constant;
using SocialMedia_Backend.Utitlities;

namespace SocialMedia_Backend.Data;

public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {

        ApplicationUser user = new ApplicationUser
        {
            Id = SequentialGuid.NewGuid(),
            UserName = "Admin",
            NormalizedUserName = "Admin",
            Email = "admin@admin.com",
            NormalizedEmail = "admin@admin.com",
        };

        var passHasher = new PasswordHasher<ApplicationUser>();
        string p = passHasher.HashPassword(user, "Asd@1234");

        user.PasswordHash = p;

        modelBuilder.Entity<ApplicationUser>().HasData(
            user
        );

        IdentityRole<Guid> Admin = new IdentityRole<Guid>
        {
            Name = Roles.ADMIN,
            Id = SequentialGuid.NewGuid(),
        };
        IdentityRole<Guid> Owner = new IdentityRole<Guid>
        {
            Name = Roles.OWNER,
            Id = SequentialGuid.NewGuid(),
        };
        IdentityRole<Guid> Agency = new IdentityRole<Guid>
        {
            Name = Roles.AGENCY,
            Id = SequentialGuid.NewGuid(),
        };

        modelBuilder.Entity<IdentityRole<Guid>>().HasData(
            Admin,
            Owner,
            Agency
        );

        modelBuilder.Entity<IdentityUserRole<Guid>>().HasData(
            new IdentityUserRole<Guid>
            {
                RoleId = Admin.Id,
                UserId = user.Id
            }
        );

        string FeatureKey = "RoleManagement";
        List<string> ClaimList = ClaimStore.GetClaims().FirstOrDefault(x => x.Key.Equals(FeatureKey)).Value.ToList();
        List<IdentityRoleClaim<Guid>> RolesList = new List<IdentityRoleClaim<Guid>>();

        for (var i = 1; i <= ClaimList.Count; i++)
        {
            RolesList.Add(new IdentityRoleClaim<Guid>
            {
                Id = i,
                RoleId = Admin.Id,
                ClaimType = FeatureKey,
                ClaimValue = ClaimList[i - 1],
            });
        }

        modelBuilder.Entity<IdentityRoleClaim<Guid>>().HasData(

            RolesList
        //new IdentityRoleClaim<Guid>
        //{
        //    Id = 2,
        //    RoleId = Admin.Id,
        //    ClaimType = ClaimStore.UpdateRoleClaim,
        //    ClaimValue = ClaimStore.UpdateRoleClaim,
        //},
        //new IdentityRoleClaim<Guid>
        //{
        //    Id = 3,
        //    RoleId = Admin.Id,
        //    ClaimType = ClaimStore.ViewRoleClaim,
        //    ClaimValue = ClaimStore.ViewRoleClaim,
        //},
        //new IdentityRoleClaim<Guid>
        //{
        //    Id = 4,
        //    RoleId = Admin.Id,
        //    ClaimType = ClaimStore.DeleteRoleClaim,
        //    ClaimValue = ClaimStore.DeleteRoleClaim,
        //},
        //new IdentityRoleClaim<Guid>
        //{
        //    Id = 5,
        //    RoleId = Admin.Id,
        //    ClaimType = ClaimStore.ManagePermissionClaim,
        //    ClaimValue = ClaimStore.ManagePermissionClaim,
        //}
        );
    }
}

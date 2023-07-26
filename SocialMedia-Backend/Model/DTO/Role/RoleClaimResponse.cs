namespace SocialMedia_Backend.Model.DTO.Role;

public class RoleClaimResponse
{
    public Guid Id { get; set; }

    public string RoleName { get; set; }

    public List<FeatureClaim> RolePermissions { get; set; } = new List<FeatureClaim>();
}

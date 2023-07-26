namespace SocialMedia_Backend.Model.DTO.Role;

public class RoleCreateRequest
{
    public string RoleName { get; set; }

    public List<FeatureClaim> RolePermissions { get; set; } = new List<FeatureClaim>();
}

public class FeatureClaim
{
    public string FeatureName { get; set; }

    public List<string> Permisssions { get; set; } = new List<string>();
}

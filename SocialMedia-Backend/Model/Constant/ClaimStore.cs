namespace SocialMedia_Backend.Model.Constant;

public static class ClaimStore
{
    #region AuthModule
    public const string RefreshTokenClaim = "RefreshToken";
    public const string ForgotPasswordTokenClaim = "ForgotPasswordToken";
    public const string ActivateAccountTokenClaim = "ActivateAccountToken";
    #endregion

    #region RoleManagement
    public const string ManageRoleClaim = "ManageRole";
    public const string ViewRoleClaim = "ViewRole";
    #endregion

    #region PostManagement
    public const string ManagePostClaim = "ManagePost";
    public const string ViewPostClaim = "ViewPost";
    #endregion

    #region EmployeeManagement
    public const string ManageEmployeeClaim = "ManageEmployee";
    public const string ViewEmployeeClaim = "ViewEmployee";
    #endregion

    public static Dictionary<string, List<string>> GetClaims()
    {
        return new Dictionary<string, List<string>>()
        {
            { "AuthManagement", new List<string> { RefreshTokenClaim , ForgotPasswordTokenClaim ,ActivateAccountTokenClaim } },
            { "RoleManagement", new List<string> {  ViewRoleClaim ,  ManageRoleClaim } },
            { "PostManagement", new List<string> { ManagePostClaim ,  ViewPostClaim } },
            { "EmployeeManagement", new List<string> { ManageEmployeeClaim, ViewEmployeeClaim } },
        };
    }

}


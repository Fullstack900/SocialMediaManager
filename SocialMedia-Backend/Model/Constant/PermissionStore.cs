namespace SocialMedia_Backend.Model.Constant;

public static class PermissionStore
{
    #region AuthModule
    public const string RefreshTokenPolicy = "RefreshTokenPermission";
    public const string ForgotPasswordTokenPolicy = "ForgotPasswordTokenPermission";
    public const string ActivateAccountTokenPolicy = "ActivateAccountTokenPermission";
    #endregion

    #region RoleManagement
    public const string ManageRolePolicy = "ManageRolePermission";
    public const string ViewRolePolicy = "ViewRolePermission";
    #endregion

    #region PostManagement
    public const string ManagePostPolicy = "ManagePostPermission";
    public const string ViewPostPolicy = "ViewPostPermission";
    #endregion

    #region EmployeeManagement
    public const string ManageEmployeePolicy = "ManageEmployeePermission";
    public const string ViewEmployeePolicy = "ViewEmployeePermission";
    #endregion

}


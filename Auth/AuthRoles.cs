namespace graphql_proj_Csharp.Auth;

public static class AuthRoles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string Customer = "Customer";
    public const string AdminOrManager = Admin + "," + Manager;

    public static readonly string[] All = [Admin, Manager, Customer];
}

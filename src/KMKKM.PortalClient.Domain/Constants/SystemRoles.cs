namespace KMKKM.PortalClient.Domain.Constants;

public static class SystemRoles
{
    public const string Admin = "Admin";
    public const string Sales = "Sales";
    public const string Support = "Support";
    public const string Client = "Client";

    public static readonly string[] AdminAreaRoles = [Admin, Sales, Support];
}

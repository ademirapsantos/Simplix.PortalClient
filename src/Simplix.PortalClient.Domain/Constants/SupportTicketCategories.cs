namespace Simplix.PortalClient.Domain.Constants;

public static class SupportTicketCategories
{
    public const string Bug = "Bug";
    public const string BusinessRule = "Regra de negocio";
    public const string Customization = "Customizacao";
    public const string Question = "Duvida";
    public const string Improvement = "Melhoria";

    public static readonly string[] All =
    [
        Bug,
        BusinessRule,
        Customization,
        Question,
        Improvement
    ];
}


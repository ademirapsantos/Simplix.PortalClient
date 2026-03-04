namespace KMKKM.PortalClient.Domain.Constants;

public static class SupportTicketStatuses
{
    public const string Created = "Criado";
    public const string Prioritized = "Priorizado";
    public const string Analyst = "Analista";
    public const string Development = "Desenvolvimento";
    public const string Customization = "Customizacao";
    public const string Homologation = "Homologacao";
    public const string Blocked = "Impedimento";
    public const string Deploy = "Deploy";
    public const string Completed = "Concluido";

    public static readonly string[] Ordered =
    [
        Created,
        Prioritized,
        Analyst,
        Development,
        Customization,
        Homologation,
        Blocked,
        Deploy,
        Completed
    ];
}

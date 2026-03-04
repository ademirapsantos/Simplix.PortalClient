namespace Simplix.PortalClient.Domain.Constants;

public static class SupportTicketPriorities
{
    public const string Low = "Baixa";
    public const string Medium = "Media";
    public const string High = "Alta";
    public const string Urgent = "Urgente";

    public static readonly string[] All =
    [
        Low,
        Medium,
        High,
        Urgent
    ];
}


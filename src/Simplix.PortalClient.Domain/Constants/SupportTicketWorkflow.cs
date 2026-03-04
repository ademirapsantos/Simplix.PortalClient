namespace Simplix.PortalClient.Domain.Constants;

public static class SupportTicketWorkflow
{
    public static bool IsValidStatus(string status) =>
        SupportTicketStatuses.Ordered.Contains(status, StringComparer.Ordinal);

    public static bool IsValidPriority(string priority) =>
        SupportTicketPriorities.All.Contains(priority, StringComparer.Ordinal);

    public static bool IsValidCategory(string category) =>
        SupportTicketCategories.All.Contains(category, StringComparer.Ordinal);

    public static bool CanClientMove(string currentStatus, string targetStatus) =>
        string.Equals(targetStatus, SupportTicketStatuses.Completed, StringComparison.Ordinal) &&
        (string.Equals(currentStatus, SupportTicketStatuses.Created, StringComparison.Ordinal) ||
         string.Equals(currentStatus, SupportTicketStatuses.Homologation, StringComparison.Ordinal));
}


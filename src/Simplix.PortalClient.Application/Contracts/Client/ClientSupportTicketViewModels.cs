namespace Simplix.PortalClient.Application.Contracts.Client;

public sealed record ClientSupportTicketBoardViewModel(
    string CustomerName,
    bool CanManageWorkflow,
    IReadOnlyCollection<ClientSupportTicketColumnViewModel> Columns);

public sealed record ClientSupportTicketColumnViewModel(
    string Status,
    string Title,
    string Description,
    IReadOnlyCollection<ClientSupportTicketCardViewModel> Cards);

public sealed record ClientSupportTicketCardViewModel(
    Guid Id,
    string ReferenceNumber,
    string Subject,
    string Category,
    string Priority,
    string Status,
    string CustomerName,
    string AssignedAgentFullName,
    DateTime SlaTargetAtUtc,
    string SlaLabel,
    string SlaTone,
    DateTime CreatedAtUtc,
    DateTime LastMovedAtUtc,
    bool CanClientMoveToCompleted);

public sealed record ClientSupportTicketDetailsViewModel(
    Guid Id,
    string ReferenceNumber,
    string Subject,
    string Description,
    string Category,
    string Priority,
    string Status,
    string CustomerName,
    string CustomerEmail,
    string AssignedAgentFullName,
    DateTime SlaTargetAtUtc,
    string SlaLabel,
    string SlaTone,
    DateTime CreatedAtUtc,
    DateTime LastMovedAtUtc,
    bool CanClientMoveToCompleted);

public sealed record CreateClientSupportTicketCommand(
    string Subject,
    string Description,
    string Category,
    string Priority);


namespace KMKKM.PortalClient.Application.Contracts.Portal;

public sealed record StoreAppCardViewModel(
    string Name,
    string Category,
    string Description,
    string Positioning,
    string Status);

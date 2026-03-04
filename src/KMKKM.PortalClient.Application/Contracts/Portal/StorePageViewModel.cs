namespace KMKKM.PortalClient.Application.Contracts.Portal;

public sealed record StorePageViewModel(
    string Title,
    string Subtitle,
    IReadOnlyCollection<string> ValueHighlights,
    IReadOnlyCollection<StoreAppCardViewModel> Applications);

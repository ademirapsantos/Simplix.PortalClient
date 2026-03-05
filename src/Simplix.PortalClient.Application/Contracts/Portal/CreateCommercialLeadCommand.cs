namespace Simplix.PortalClient.Application.Contracts.Portal;

public sealed record CreateCommercialLeadCommand(
    string ContactName,
    string CompanyName,
    string Email,
    string Phone,
    string InterestedProduct,
    string Message);


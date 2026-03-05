using Simplix.PortalClient.Application.Contracts.Portal;

namespace Simplix.PortalClient.Web.Models.Portal;

public sealed record StorePageScreenViewModel(
    StorePageViewModel Store,
    CommercialLeadFormViewModel LeadForm);


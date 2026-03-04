using KMKKM.PortalClient.Application.Contracts.Portal;

namespace KMKKM.PortalClient.Web.Models.Portal;

public sealed record StorePageScreenViewModel(
    StorePageViewModel Store,
    CommercialLeadFormViewModel LeadForm);

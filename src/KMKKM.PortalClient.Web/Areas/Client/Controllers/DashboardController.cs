using KMKKM.PortalClient.Domain.Constants;
using KMKKM.PortalClient.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMKKM.PortalClient.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = $"{SystemRoles.Admin},{SystemRoles.Client}")]
public sealed class DashboardController : Controller
{
    private readonly IClientWorkspaceService _clientWorkspaceService;

    public DashboardController(IClientWorkspaceService clientWorkspaceService)
    {
        _clientWorkspaceService = clientWorkspaceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var viewModel = await _clientWorkspaceService.GetWorkspaceAsync(cancellationToken);
        return View(viewModel);
    }
}

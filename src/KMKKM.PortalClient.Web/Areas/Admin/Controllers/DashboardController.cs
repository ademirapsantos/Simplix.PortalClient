using KMKKM.PortalClient.Domain.Constants;
using KMKKM.PortalClient.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMKKM.PortalClient.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = $"{SystemRoles.Admin},{SystemRoles.Sales},{SystemRoles.Support}")]
public sealed class DashboardController : Controller
{
    private readonly IAdminWorkspaceService _adminWorkspaceService;

    public DashboardController(IAdminWorkspaceService adminWorkspaceService)
    {
        _adminWorkspaceService = adminWorkspaceService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ViewBag.CanManageSystemUpdate = User.IsInRole(SystemRoles.Admin);
        var viewModel = await _adminWorkspaceService.GetWorkspaceAsync(cancellationToken);
        return View(viewModel);
    }
}

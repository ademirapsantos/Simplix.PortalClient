using KMKKM.PortalClient.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace KMKKM.PortalClient.Web.Areas.Admin.Controllers;

[Area("Admin")]
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
        var viewModel = await _adminWorkspaceService.GetWorkspaceAsync(cancellationToken);
        return View(viewModel);
    }
}

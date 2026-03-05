using Simplix.PortalClient.Application.Interfaces;
using Simplix.PortalClient.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simplix.PortalClient.Web.Areas.Admin.Controllers;

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

    [HttpPost]
    [Authorize(Roles = SystemRoles.Admin)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> StartUpdate(CancellationToken cancellationToken)
    {
        var result = await _adminWorkspaceService.StartSystemUpdateAsync(cancellationToken);
        TempData[result.Success ? "UpdateSuccess" : "UpdateError"] = result.Message;
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = SystemRoles.Admin)]
    public async Task<IActionResult> UpdateRuntimeStatus(CancellationToken cancellationToken)
    {
        var status = await _adminWorkspaceService.GetSystemUpdateRuntimeStatusAsync(cancellationToken);
        return Json(status);
    }
}

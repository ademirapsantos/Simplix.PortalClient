using Simplix.PortalClient.Domain.Constants;
using Simplix.PortalClient.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Simplix.PortalClient.Web.Areas.Client.Controllers;

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
        string userEmail = User.Identity?.Name ?? string.Empty;
        bool canViewAll = User.IsInRole(SystemRoles.Admin);
        var viewModel = await _clientWorkspaceService.GetWorkspaceAsync(userEmail, canViewAll, cancellationToken);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Subscriptions(CancellationToken cancellationToken)
    {
        string userEmail = User.Identity?.Name ?? string.Empty;
        bool canViewAll = User.IsInRole(SystemRoles.Admin);
        var viewModel = await _clientWorkspaceService.GetWorkspaceAsync(userEmail, canViewAll, cancellationToken);
        return View(viewModel);
    }

    [HttpGet]
    public async Task<IActionResult> Finance(CancellationToken cancellationToken)
    {
        string userEmail = User.Identity?.Name ?? string.Empty;
        bool canViewAll = User.IsInRole(SystemRoles.Admin);
        var viewModel = await _clientWorkspaceService.GetWorkspaceAsync(userEmail, canViewAll, cancellationToken);
        return View(viewModel);
    }
}


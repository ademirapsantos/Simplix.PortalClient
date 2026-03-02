using System.Diagnostics;
using KMKKM.PortalClient.Application.Contracts.Portal;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace KMKKM.PortalClient.Web.Controllers;

public class HomeController : Controller
{
    private readonly IPortalExperienceService _portalExperienceService;
    private readonly ILogger<HomeController> _logger;

    public HomeController(
        IPortalExperienceService portalExperienceService,
        ILogger<HomeController> logger)
    {
        _portalExperienceService = portalExperienceService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        LandingPageViewModel viewModel = await _portalExperienceService.GetLandingPageAsync(cancellationToken);
        return View(viewModel);
    }

    [HttpGet("store")]
    public async Task<IActionResult> Store(CancellationToken cancellationToken)
    {
        StorePageViewModel viewModel = await _portalExperienceService.GetStorePageAsync(cancellationToken);
        return View(viewModel);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Unhandled request error for trace identifier {TraceIdentifier}.", HttpContext.TraceIdentifier);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

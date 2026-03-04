using System.Diagnostics;
using Simplix.PortalClient.Application.Contracts.Portal;
using Simplix.PortalClient.Application.Interfaces;
using Simplix.PortalClient.Web.Models;
using Simplix.PortalClient.Web.Models.Portal;
using Microsoft.AspNetCore.Mvc;

namespace Simplix.PortalClient.Web.Controllers;

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
        return View(new StorePageScreenViewModel(viewModel, new CommercialLeadFormViewModel()));
    }

    [HttpPost("store/lead")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLead(CommercialLeadFormViewModel model, CancellationToken cancellationToken)
    {
        StorePageViewModel storeViewModel = await _portalExperienceService.GetStorePageAsync(cancellationToken);

        if (!ModelState.IsValid)
        {
            return View("Store", new StorePageScreenViewModel(storeViewModel, model));
        }

        await _portalExperienceService.RegisterCommercialLeadAsync(
            new CreateCommercialLeadCommand(
                model.ContactName,
                model.CompanyName,
                model.Email,
                model.Phone,
                model.InterestedProduct,
                model.Message),
            cancellationToken);

        TempData["StoreLeadCreated"] = true;
        return RedirectToAction(nameof(Store));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Unhandled request error for trace identifier {TraceIdentifier}.", HttpContext.TraceIdentifier);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


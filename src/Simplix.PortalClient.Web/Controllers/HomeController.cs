using System.Diagnostics;
using System.Text;
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
        return View(viewModel);
    }

    [HttpGet("store/interesse-comercial")]
    public IActionResult CommercialInterest()
    {
        return View(new CommercialLeadFormViewModel());
    }

    [HttpGet("store/chat")]
    public IActionResult StoreChat()
    {
        return View();
    }

    [HttpPost("store/lead")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateLead(CommercialLeadFormViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View("CommercialInterest", model);
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
        return RedirectToAction(nameof(CommercialInterest));
    }

    [HttpPost("store/chat/message")]
    public async Task<IActionResult> StoreChatMessage([FromBody] StoreChatMessageInputModel model, CancellationToken cancellationToken)
    {
        if (model is null || string.IsNullOrWhiteSpace(model.Message))
        {
            return BadRequest(new { error = "Informe uma mensagem." });
        }

        StorePageViewModel storeViewModel = await _portalExperienceService.GetStorePageAsync(cancellationToken);
        string answer = BuildStoreAssistantReply(model.Message, storeViewModel);

        return Ok(new StoreChatMessageResponseModel(answer, DateTimeOffset.UtcNow));
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        _logger.LogError("Unhandled request error for trace identifier {TraceIdentifier}.", HttpContext.TraceIdentifier);
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private static string BuildStoreAssistantReply(string message, StorePageViewModel storeViewModel)
    {
        string normalized = message.Trim().ToLowerInvariant();
        StoreAppCardViewModel? matchedApplication = storeViewModel.Applications
            .FirstOrDefault(x => normalized.Contains(x.Name.ToLowerInvariant()));

        if (matchedApplication is not null)
        {
            return $"O {matchedApplication.Name} esta em {matchedApplication.Status}. {matchedApplication.Positioning} Se quiser, eu ja te levo para o formulario de interesse comercial.";
        }

        if (normalized.Contains("preco") || normalized.Contains("valor") || normalized.Contains("plano"))
        {
            return "A Store apresenta os produtos e o posicionamento comercial. Para proposta de preco e escopo, clique em 'Enviar interesse comercial' para o time de vendas te atender com valores.";
        }

        if (normalized.Contains("suporte") || normalized.Contains("implant") || normalized.Contains("atendimento"))
        {
            return "A equipe simplix cobre descoberta, implantacao e suporte continuo. Posso te ajudar a identificar o produto mais aderente e, em seguida, voce envia o interesse comercial.";
        }

        var builder = new StringBuilder();
        builder.Append("Posso te ajudar com os produtos da Store. Hoje temos: ");
        builder.Append(string.Join(", ", storeViewModel.Applications.Select(x => x.Name)));
        builder.Append(". Me diga o nome do produto que voce quer avaliar.");
        return builder.ToString();
    }
}

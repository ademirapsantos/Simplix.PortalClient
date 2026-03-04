using KMKKM.PortalClient.Application.Contracts.Client;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Domain.Constants;
using KMKKM.PortalClient.Web.Models.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KMKKM.PortalClient.Web.Areas.Client.Controllers;

[Area("Client")]
[Authorize(Roles = $"{SystemRoles.Admin},{SystemRoles.Client}")]
public sealed class SupportTicketsController : Controller
{
    private readonly IClientSupportTicketService _clientSupportTicketService;

    public SupportTicketsController(IClientSupportTicketService clientSupportTicketService)
    {
        _clientSupportTicketService = clientSupportTicketService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        ClientSupportTicketBoardViewModel viewModel = await _clientSupportTicketService.GetBoardAsync(
            GetUserEmail(),
            CanManageWorkflow(),
            cancellationToken);

        return View(viewModel);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSupportTicketViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSupportTicketViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _clientSupportTicketService.CreateTicketAsync(
            GetUserEmail(),
            new CreateClientSupportTicketCommand(model.Subject, model.Description, model.Category, model.Priority),
            cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid ticketId, CancellationToken cancellationToken)
    {
        ClientSupportTicketDetailsViewModel? viewModel = await _clientSupportTicketService.GetTicketAsync(
            ticketId,
            GetUserEmail(),
            CanManageWorkflow(),
            cancellationToken);

        return viewModel is null ? NotFound() : View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Move(Guid ticketId, string targetStatus, CancellationToken cancellationToken)
    {
        await _clientSupportTicketService.MoveTicketAsync(
            ticketId,
            targetStatus,
            GetUserEmail(),
            CanManageWorkflow(),
            cancellationToken);

        return RedirectToAction(nameof(Index));
    }

    private string GetUserEmail() => User.Identity?.Name ?? string.Empty;

    private bool CanManageWorkflow() => User.IsInRole(SystemRoles.Admin);
}

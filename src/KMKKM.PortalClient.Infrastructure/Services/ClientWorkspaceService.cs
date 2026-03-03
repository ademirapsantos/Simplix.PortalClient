using KMKKM.PortalClient.Application.Contracts.Client;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class ClientWorkspaceService : IClientWorkspaceService
{
    private readonly PortalClientDbContext _dbContext;

    public ClientWorkspaceService(PortalClientDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ClientWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken)
    {
        var license = await _dbContext.CustomerLicenses
            .AsNoTracking()
            .OrderBy(x => x.CustomerName)
            .FirstAsync(cancellationToken);

        var activeProducts = await _dbContext.CustomerLicenses
            .AsNoTracking()
            .Where(x => x.CustomerName == license.CustomerName)
            .Select(x => x.ProductName)
            .ToArrayAsync(cancellationToken);

        var availableUpgrades = await _dbContext.Products
            .AsNoTracking()
            .Where(x => x.SupportsUpgrade)
            .Select(x => $"Upgrade para {x.Name}")
            .ToArrayAsync(cancellationToken);

        var recentTickets = await _dbContext.SupportTickets
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => $"{x.Subject} | {x.Status}")
            .Take(5)
            .ToArrayAsync(cancellationToken);

        var viewModel = new ClientWorkspaceViewModel(
            license.CustomerName,
            activeProducts,
            availableUpgrades,
            [$"Renovar licenca {license.PlanName} ate {license.ExpiresOn:dd/MM}", $"Validar produto {license.ProductName} apos onboarding"],
            recentTickets);

        return viewModel;
    }
}

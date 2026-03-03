using KMKKM.PortalClient.Application.Contracts.Portal;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class PortalExperienceService : IPortalExperienceService
{
    private static readonly IReadOnlyCollection<RoadmapItemViewModel> Roadmap =
    [
        new(1, "Portal publico + Store", "Apresentacao da startup, portfolio, catalogo de apps e captura de interesse comercial."),
        new(2, "Area do cliente", "Painel para licencas, upgrades, historico financeiro e acesso aos produtos contratados."),
        new(3, "Chamados e atendimento", "Ordem de servico, suporte a bugs, regras de processo, customizacoes e chat."),
        new(4, "Admin comercial e suporte", "CRM, metricas de venda, filas operacionais e controle da experiencia do cliente.")
    ];

    private readonly PortalClientDbContext _dbContext;

    public PortalExperienceService(PortalClientDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<LandingPageViewModel> GetLandingPageAsync(CancellationToken cancellationToken)
    {
        var applications = await _dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new StoreAppCardViewModel(x.Name, x.Category, x.Description, x.Positioning, x.Status))
            .ToArrayAsync(cancellationToken);

        var plans = await _dbContext.CommercialPlans
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new CommercialPlanViewModel(x.Name, x.Description, x.TargetAudience))
            .ToArrayAsync(cancellationToken);

        var viewModel = new LandingPageViewModel(
            "K.M.K.K.M",
            "K.M.K.K.M Portal do Cliente",
            "Um ecossistema unico para apresentar a startup, vender licencas, gerenciar clientes e organizar suporte comercial e tecnico.",
            "A K.M.K.K.M Store nasce como camada publica do negocio e o Portal do Cliente como operacao central de relacionamento, upgrades, suporte e atendimento.",
            [
                "Sem login na pagina inicial para fortalecer marca e portfolio.",
                "Store com exibicao dos apps e comercializacao de licencas.",
                "Area do cliente focada em valor pos-venda, upgrades e suporte.",
                "Admin unico com perfis para dono, vendas e equipe de suporte."
            ],
            applications.Take(2).ToArray(),
            plans,
            Roadmap);

        return viewModel;
    }

    public async Task<StorePageViewModel> GetStorePageAsync(CancellationToken cancellationToken)
    {
        var applications = await _dbContext.Products
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new StoreAppCardViewModel(x.Name, x.Category, x.Description, x.Positioning, x.Status))
            .ToArrayAsync(cancellationToken);

        var viewModel = new StorePageViewModel(
            "K.M.K.K.M Store",
            "Catalogo institucional dos aplicativos da startup. Nesta fase inicial a Store apresenta os produtos, posicionamento e status comercial.",
            applications);

        return viewModel;
    }
}

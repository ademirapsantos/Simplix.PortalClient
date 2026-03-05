using Simplix.PortalClient.Application.Contracts.Admin;
using Simplix.PortalClient.Application.Interfaces;
using Simplix.PortalClient.Infrastructure.Options;
using Simplix.PortalClient.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Simplix.PortalClient.Infrastructure.Services;

internal sealed class AdminWorkspaceService : IAdminWorkspaceService
{
    private readonly PortalClientDbContext _dbContext;
    private readonly OpenClawOptions _openClawOptions;
    private readonly UpdateServerClient _updateServerClient;

    public AdminWorkspaceService(
        PortalClientDbContext dbContext,
        UpdateServerClient updateServerClient,
        IOptions<OpenClawOptions> openClawOptions)
    {
        _dbContext = dbContext;
        _updateServerClient = updateServerClient;
        _openClawOptions = openClawOptions.Value;
    }

    public async Task<AdminWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken)
    {
        int totalProducts = await _dbContext.Products.CountAsync(cancellationToken);
        int totalLicenses = await _dbContext.CustomerLicenses.CountAsync(cancellationToken);
        int openTickets = await _dbContext.SupportTickets.CountAsync(x => x.Status != "Fechado", cancellationToken);
        SystemUpdateStatusViewModel systemUpdateStatus = await _updateServerClient.GetStatusAsync(cancellationToken);

        var viewModel = new AdminWorkspaceViewModel(
            "Painel central da operacao Simplix",
            [
                new("Produtos catalogados", totalProducts.ToString(), "Quantidade atual de aplicacoes estruturadas na Store."),
                new("Clientes ativos", totalLicenses.ToString(), "Base inicial para assinatura, upgrades e recorrencia."),
                new("Pagamentos em atraso", "0", "Indicador provisoriamente manual ate entrar o modulo financeiro.")
            ],
            [
                new("Chamados abertos", openTickets.ToString(), "Fila somando bugs, regras de processo e customizacoes."),
                new("Tempo medio de resposta", "18 min", "Indicador previsto para o atendimento instantaneo."),
                new("OS em aprovacao", "1", "Demanda inicial aguardando aceite comercial ou tecnico.")
            ],
            ["Fila comercial de recuperacao", "Fila de suporte critico", "Pipeline de novas ofertas"],
            BuildOpenClawStatus(),
            systemUpdateStatus);

        return viewModel;
    }

    public Task<SystemUpdateTriggerResultViewModel> StartSystemUpdateAsync(CancellationToken cancellationToken)
    {
        return _updateServerClient.StartUpdateAsync(cancellationToken);
    }

    public Task<SystemUpdateRuntimeStatusViewModel> GetSystemUpdateRuntimeStatusAsync(CancellationToken cancellationToken)
    {
        return _updateServerClient.GetRuntimeStatusAsync(cancellationToken);
    }

    private OpenClawStatusViewModel BuildOpenClawStatus()
    {
        string mode = _openClawOptions.EnableTicketAutomation ? "Integracao habilitada" : "Integracao preparada";
        string summary = _openClawOptions.EnableTicketAutomation
            ? $"OpenClaw apontando para {_openClawOptions.BaseUrl} com automacao de chamados ativa."
            : "A configuracao base foi separada para automatizar atendimento, triagem e respostas de chamados em fase posterior.";

        return new OpenClawStatusViewModel(
            mode,
            summary,
            [
                "Definir escopo da API do OpenClaw para chat e chamados.",
                "Persistir tickets, mensagens e status em PostgreSQL.",
                "Acoplar automacoes apenas apos consolidar o fluxo manual do suporte."
            ]);
    }
}

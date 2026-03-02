using KMKKM.PortalClient.Application.Contracts.Admin;
using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class AdminWorkspaceService : IAdminWorkspaceService
{
    private readonly OpenClawOptions _openClawOptions;

    public AdminWorkspaceService(IOptions<OpenClawOptions> openClawOptions)
    {
        _openClawOptions = openClawOptions.Value;
    }

    public Task<AdminWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken)
    {
        var viewModel = new AdminWorkspaceViewModel(
            "Painel central da operacao K.M.K.K.M",
            [
                new("Leads em analise", "32", "Interessados que navegaram na Store e ainda nao converteram."),
                new("Clientes ativos", "14", "Base inicial para assinatura, upgrades e recorrencia."),
                new("Pagamentos em atraso", "3", "Clientes que precisam de acao comercial ou renegociacao.")
            ],
            [
                new("Chamados abertos", "11", "Fila somando bugs, regras de processo e customizacoes."),
                new("Tempo medio de resposta", "18 min", "Indicador previsto para o atendimento instantaneo."),
                new("OS em aprovacao", "4", "Demandas aguardando aceite comercial ou tecnico.")
            ],
            ["Fila comercial de recuperacao", "Fila de suporte critico", "Pipeline de novas ofertas"],
            BuildOpenClawStatus());

        return Task.FromResult(viewModel);
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

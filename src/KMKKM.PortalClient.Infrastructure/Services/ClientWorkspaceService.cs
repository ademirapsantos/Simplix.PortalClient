using KMKKM.PortalClient.Application.Contracts.Client;
using KMKKM.PortalClient.Application.Interfaces;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class ClientWorkspaceService : IClientWorkspaceService
{
    public Task<ClientWorkspaceViewModel> GetWorkspaceAsync(CancellationToken cancellationToken)
    {
        var viewModel = new ClientWorkspaceViewModel(
            "Cliente demonstracao",
            ["KMKKM Finance"],
            ["Upgrade para modulo de relatorios gerenciais", "Pacote de atendimento prioritario"],
            ["Renovar licenca anual ate 15/03", "Validar implantacao do modulo financeiro"],
            ["OS-1042 | Correcao de regra fiscal", "OS-1048 | Novo campo em cadastro de clientes"]);

        return Task.FromResult(viewModel);
    }
}

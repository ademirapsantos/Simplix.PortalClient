using KMKKM.PortalClient.Application.Contracts.Portal;
using KMKKM.PortalClient.Application.Interfaces;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class PortalExperienceService : IPortalExperienceService
{
    private static readonly IReadOnlyCollection<StoreAppCardViewModel> Applications =
    [
        new("KMKKM Finance", "ERP Financeiro", "Gestao de faturamento, recebimentos, indicadores e cobranca para operacoes de servico.", "Licenciamento anual e upgrades por modulo.", "Em destaque"),
        new("KMKKM Service Desk", "Suporte e OS", "Chamados, ordem de servico, SLA, base de conhecimento e trilha completa de atendimento.", "Ideal para contratos com suporte recorrente.", "Planejado para fase 2"),
        new("KMKKM Sales Cloud", "CRM Comercial", "Funil de leads, ofertas, descontos, promocoes, clientes em atraso e analise de conversao.", "Voltado para equipe comercial e crescimento da carteira.", "Planejado para fase 3")
    ];

    private static readonly IReadOnlyCollection<CommercialPlanViewModel> Plans =
    [
        new("Start", "Entrada com implantacao mais licenca base do sistema escolhido.", "Pequenas operacoes que querem padronizar o atendimento."),
        new("Growth", "Licenca recorrente, suporte prioritario e possibilidade de upgrade por modulo.", "Clientes que precisam escalar sem trocar de plataforma."),
        new("Enterprise", "Pacote com customizacao, SLA dedicado e integracoes sob demanda.", "Operacoes com processos proprios e mais volume.")
    ];

    private static readonly IReadOnlyCollection<RoadmapItemViewModel> Roadmap =
    [
        new(1, "Portal publico + Store", "Apresentacao da startup, portfolio, catalogo de apps e captura de interesse comercial."),
        new(2, "Area do cliente", "Painel para licencas, upgrades, historico financeiro e acesso aos produtos contratados."),
        new(3, "Chamados e atendimento", "Ordem de servico, suporte a bugs, regras de processo, customizacoes e chat."),
        new(4, "Admin comercial e suporte", "CRM, metricas de venda, filas operacionais e controle da experiencia do cliente.")
    ];

    public Task<LandingPageViewModel> GetLandingPageAsync(CancellationToken cancellationToken)
    {
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
            Applications.Take(2).ToArray(),
            Plans,
            Roadmap);

        return Task.FromResult(viewModel);
    }

    public Task<StorePageViewModel> GetStorePageAsync(CancellationToken cancellationToken)
    {
        var viewModel = new StorePageViewModel(
            "K.M.K.K.M Store",
            "Catalogo institucional dos aplicativos da startup. Nesta fase inicial a Store apresenta os produtos, posicionamento e status comercial.",
            Applications);

        return Task.FromResult(viewModel);
    }
}

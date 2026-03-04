using Simplix.PortalClient.Domain.Constants;
using Simplix.PortalClient.Domain.Entities;
using Simplix.PortalClient.Infrastructure.Identity;
using Simplix.PortalClient.Infrastructure.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Simplix.PortalClient.Infrastructure.Persistence;

internal sealed class PortalClientDbContextInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<PortalClientDbContextInitializer> _logger;
    private readonly SeedUsersOptions _seedUsersOptions;

    public PortalClientDbContextInitializer(
        IServiceProvider serviceProvider,
        ILogger<PortalClientDbContextInitializer> logger,
        IOptions<SeedUsersOptions> seedUsersOptions)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _seedUsersOptions = seedUsersOptions.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<PortalClientDbContext>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        _logger.LogInformation("Ensuring PostgreSQL schema is available.");
        await dbContext.Database.EnsureCreatedAsync(cancellationToken);
        await SeedAsync(dbContext, cancellationToken);
        await SeedIdentityAsync(roleManager, userManager);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static async Task SeedAsync(PortalClientDbContext dbContext, CancellationToken cancellationToken)
    {
        if (!await dbContext.Products.AnyAsync(cancellationToken))
        {
            dbContext.Products.AddRange(
                new Product
                {
                    Name = "Simplix Finance",
                    Category = "ERP Financeiro",
                    Description = "Gestao de faturamento, recebimentos, indicadores e cobranca para operacoes de servico.",
                    Positioning = "Licenciamento anual e upgrades por modulo.",
                    Status = "Em destaque",
                    SupportsUpgrade = true
                },
                new Product
                {
                    Name = "Simplix Service Desk",
                    Category = "Suporte e OS",
                    Description = "Chamados, ordem de servico, SLA, base de conhecimento e trilha completa de atendimento.",
                    Positioning = "Ideal para contratos com suporte recorrente.",
                    Status = "Planejado para fase 2",
                    SupportsUpgrade = true
                },
                new Product
                {
                    Name = "Simplix Sales Cloud",
                    Category = "CRM Comercial",
                    Description = "Funil de leads, ofertas, descontos, promocoes, clientes em atraso e analise de conversao.",
                    Positioning = "Voltado para equipe comercial e crescimento da carteira.",
                    Status = "Planejado para fase 3",
                    SupportsUpgrade = true
                });
        }

        if (!await dbContext.CommercialPlans.AnyAsync(cancellationToken))
        {
            dbContext.CommercialPlans.AddRange(
                new CommercialPlan
                {
                    Name = "Start",
                    Description = "Entrada com implantacao mais licenca base do sistema escolhido.",
                    TargetAudience = "Pequenas operacoes que querem padronizar o atendimento."
                },
                new CommercialPlan
                {
                    Name = "Growth",
                    Description = "Licenca recorrente, suporte prioritario e possibilidade de upgrade por modulo.",
                    TargetAudience = "Clientes que precisam escalar sem trocar de plataforma."
                },
                new CommercialPlan
                {
                    Name = "Enterprise",
                    Description = "Pacote com customizacao, SLA dedicado e integracoes sob demanda.",
                    TargetAudience = "Operacoes com processos proprios e mais volume."
                });
        }

        if (!await dbContext.CustomerLicenses.AnyAsync(cancellationToken))
        {
            dbContext.CustomerLicenses.AddRange(
                new CustomerLicense
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Finance",
                    PlanName = "Growth",
                    ExpiresOn = new DateOnly(2026, 3, 15)
                },
                new CustomerLicense
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Service Desk",
                    PlanName = "Start",
                    ExpiresOn = new DateOnly(2026, 4, 10)
                });
        }

        if (!await dbContext.CustomerFinancialRecords.AnyAsync(cancellationToken))
        {
            dbContext.CustomerFinancialRecords.AddRange(
                new CustomerFinancialRecord
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Finance",
                    ReferenceNumber = "FAT-2026-001",
                    Description = "Mensalidade da licenca Growth",
                    Status = "Pago",
                    PaymentMethod = "PIX",
                    Amount = 890.00m,
                    DueOn = new DateOnly(2026, 1, 10),
                    PaidOn = new DateOnly(2026, 1, 8)
                },
                new CustomerFinancialRecord
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Finance",
                    ReferenceNumber = "FAT-2026-002",
                    Description = "Mensalidade da licenca Growth",
                    Status = "Pago",
                    PaymentMethod = "Boleto",
                    Amount = 890.00m,
                    DueOn = new DateOnly(2026, 2, 10),
                    PaidOn = new DateOnly(2026, 2, 11)
                },
                new CustomerFinancialRecord
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Finance",
                    ReferenceNumber = "FAT-2026-003",
                    Description = "Mensalidade da licenca Growth",
                    Status = "Pendente",
                    PaymentMethod = "Boleto",
                    Amount = 890.00m,
                    DueOn = new DateOnly(2026, 3, 10)
                },
                new CustomerFinancialRecord
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Service Desk",
                    ReferenceNumber = "FAT-2026-004",
                    Description = "Implantacao do modulo de suporte",
                    Status = "Vencido",
                    PaymentMethod = "Transferencia",
                    Amount = 1450.00m,
                    DueOn = new DateOnly(2026, 2, 25)
                });
        }

        if (!await dbContext.CustomerProductAccesses.AnyAsync(cancellationToken))
        {
            dbContext.CustomerProductAccesses.AddRange(
                new CustomerProductAccess
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Finance",
                    EnvironmentName = "Producao",
                    AccessLabel = "Abrir ERP Financeiro",
                    AccessUrl = "https://finance.simplix.local",
                    AccessStatus = "Liberado",
                    CredentialHint = "Use o mesmo email do portal e a senha corporativa atual."
                },
                new CustomerProductAccess
                {
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    ProductName = "Simplix Service Desk",
                    EnvironmentName = "Homologacao",
                    AccessLabel = "Entrar no ambiente de testes",
                    AccessUrl = "https://service-desk-hml.simplix.local",
                    AccessStatus = "Em implantacao",
                    CredentialHint = "A liberacao final depende da baixa da implantacao e validacao do time Simplix."
                });
        }

        if (!await dbContext.SupportTickets.AnyAsync(cancellationToken))
        {
            dbContext.SupportTickets.AddRange(
                new SupportTicket
                {
                    ReferenceNumber = "17482031",
                    Subject = "Correcao de regra fiscal",
                    Description = "A regra fiscal precisa ser revisada no fechamento mensal para o cliente demonstracao.",
                    Category = SupportTicketCategories.Bug,
                    Priority = SupportTicketPriorities.High,
                    Status = SupportTicketStatuses.Development,
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    CreatedByEmail = "cliente@simplix.local",
                    AssignedAgentFullName = "Ademir Santos",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-2),
                    LastMovedAtUtc = DateTime.UtcNow.AddDays(-1),
                    SlaTargetAtUtc = DateTime.UtcNow.AddHours(10)
                },
                new SupportTicket
                {
                    ReferenceNumber = "28473105",
                    Subject = "Novo campo em cadastro de clientes",
                    Description = "Cliente solicitou incluir um campo adicional no cadastro para refletir o processo interno.",
                    Category = SupportTicketCategories.Customization,
                    Priority = SupportTicketPriorities.Medium,
                    Status = SupportTicketStatuses.Homologation,
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    CreatedByEmail = "cliente@simplix.local",
                    AssignedAgentFullName = "Ademir Santos",
                    CreatedAtUtc = DateTime.UtcNow.AddDays(-1),
                    LastMovedAtUtc = DateTime.UtcNow.AddHours(-8),
                    SlaTargetAtUtc = DateTime.UtcNow.AddHours(18)
                },
                new SupportTicket
                {
                    ReferenceNumber = "59316428",
                    Subject = "Duvida sobre fechamento financeiro",
                    Description = "Cliente precisa confirmar o fluxo esperado de fechamento antes de seguir com a operacao.",
                    Category = SupportTicketCategories.BusinessRule,
                    Priority = SupportTicketPriorities.Low,
                    Status = SupportTicketStatuses.Created,
                    CustomerName = "Cliente demonstracao",
                    CustomerEmail = "cliente@simplix.local",
                    CreatedByEmail = "cliente@simplix.local",
                    AssignedAgentFullName = "Aguardando triagem",
                    CreatedAtUtc = DateTime.UtcNow.AddHours(-12),
                    LastMovedAtUtc = DateTime.UtcNow.AddHours(-12),
                    SlaTargetAtUtc = DateTime.UtcNow.AddHours(36)
                });
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private async Task SeedIdentityAsync(
        RoleManager<ApplicationRole> roleManager,
        UserManager<ApplicationUser> userManager)
    {
        await EnsureRoleAsync(roleManager, SystemRoles.Admin);
        await EnsureRoleAsync(roleManager, SystemRoles.Sales);
        await EnsureRoleAsync(roleManager, SystemRoles.Support);
        await EnsureRoleAsync(roleManager, SystemRoles.Client);

        await EnsureUserAsync(userManager, _seedUsersOptions.Admin, SystemRoles.Admin);
        await EnsureUserAsync(userManager, _seedUsersOptions.Sales, SystemRoles.Sales);
        await EnsureUserAsync(userManager, _seedUsersOptions.Support, SystemRoles.Support);
        await EnsureUserAsync(userManager, _seedUsersOptions.Client, SystemRoles.Client);
    }

    private static async Task EnsureRoleAsync(RoleManager<ApplicationRole> roleManager, string roleName)
    {
        if (await roleManager.RoleExistsAsync(roleName))
        {
            return;
        }

        await roleManager.CreateAsync(new ApplicationRole
        {
            Name = roleName,
            NormalizedName = roleName.ToUpperInvariant()
        });
    }

    private static async Task EnsureUserAsync(
        UserManager<ApplicationUser> userManager,
        SeedUserCredentialsOptions credentials,
        string roleName)
    {
        if (string.IsNullOrWhiteSpace(credentials.Email))
        {
            return;
        }

        var user = await userManager.FindByEmailAsync(credentials.Email);
        if (user is null)
        {
            if (string.IsNullOrWhiteSpace(credentials.Password))
            {
                throw new InvalidOperationException(
                    $"Seed user '{credentials.Email}' is missing a password. Configure it via environment variable or user secrets.");
            }

            user = new ApplicationUser
            {
                Id = Guid.NewGuid(),
                FullName = credentials.FullName,
                UserName = credentials.Email,
                Email = credentials.Email,
                NormalizedEmail = credentials.Email.ToUpperInvariant(),
                NormalizedUserName = credentials.Email.ToUpperInvariant(),
                EmailConfirmed = true,
                IsActive = true
            };

            IdentityResult createResult = await userManager.CreateAsync(user, credentials.Password);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException($"Unable to create seed user '{credentials.Email}': {string.Join(", ", createResult.Errors.Select(x => x.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, roleName))
        {
            await userManager.AddToRoleAsync(user, roleName);
        }
    }
}


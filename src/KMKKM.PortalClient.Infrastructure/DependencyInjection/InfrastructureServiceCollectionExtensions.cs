using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Infrastructure.Options;
using KMKKM.PortalClient.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KMKKM.PortalClient.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.SectionName));
        services.Configure<OpenClawOptions>(configuration.GetSection(OpenClawOptions.SectionName));
        services.Configure<UpdateServerOptions>(configuration.GetSection(UpdateServerOptions.SectionName));

        services.AddScoped<IPortalExperienceService, PortalExperienceService>();
        services.AddScoped<IClientWorkspaceService, ClientWorkspaceService>();
        services.AddScoped<IAdminWorkspaceService, AdminWorkspaceService>();

        return services;
    }
}

using KMKKM.PortalClient.Application.Interfaces;
using KMKKM.PortalClient.Infrastructure.Identity;
using KMKKM.PortalClient.Infrastructure.Options;
using KMKKM.PortalClient.Infrastructure.Persistence;
using KMKKM.PortalClient.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
        services.Configure<SeedUsersOptions>(configuration.GetSection(SeedUsersOptions.SectionName));

        string connectionString = configuration.GetConnectionString("Postgres")
            ?? throw new InvalidOperationException("Connection string 'Postgres' was not configured.");

        services.AddDbContext<PortalClientDbContext>(options =>
        {
            options.UseNpgsql(connectionString, npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(3);
            });
        });

        services.AddHttpClient<UpdateServerClient>((serviceProvider, client) =>
        {
            var updateOptions = serviceProvider
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<UpdateServerOptions>>()
                .Value;

            if (!string.IsNullOrWhiteSpace(updateOptions.BaseUrl))
            {
                client.BaseAddress = new Uri(updateOptions.BaseUrl.TrimEnd('/') + "/");
            }

            client.Timeout = TimeSpan.FromSeconds(10);
        });

        services
            .AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
            })
            .AddEntityFrameworkStores<PortalClientDbContext>()
            .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "KMKKM.PortalClient.Auth";
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/account/access-denied";
            options.SlidingExpiration = true;
            options.Events = new CookieAuthenticationEvents
            {
                OnRedirectToAccessDenied = context =>
                {
                    context.Response.Redirect(options.AccessDeniedPath);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddScoped<IPortalExperienceService, PortalExperienceService>();
        services.AddScoped<IClientWorkspaceService, ClientWorkspaceService>();
        services.AddScoped<IAdminWorkspaceService, AdminWorkspaceService>();
        services.AddHostedService<PortalClientDbContextInitializer>();

        return services;
    }
}

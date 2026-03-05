using Simplix.PortalClient.Application.Interfaces;
using Simplix.PortalClient.Infrastructure.Identity;
using Simplix.PortalClient.Infrastructure.Options;
using Simplix.PortalClient.Infrastructure.Persistence;
using Simplix.PortalClient.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Simplix.PortalClient.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.SectionName));
        services.Configure<OpenClawOptions>(configuration.GetSection(OpenClawOptions.SectionName));
        services.Configure<UpdateServerOptions>(configuration.GetSection(UpdateServerOptions.SectionName));
        services.Configure<SeedUsersOptions>(configuration.GetSection(SeedUsersOptions.SectionName));
        services.Configure<DatabaseInitializationOptions>(configuration.GetSection(DatabaseInitializationOptions.SectionName));
        services.Configure<PasswordResetOptions>(configuration.GetSection(PasswordResetOptions.SectionName));

        PasswordResetOptions passwordResetOptions =
            configuration.GetSection(PasswordResetOptions.SectionName).Get<PasswordResetOptions>() ?? new PasswordResetOptions();

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
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequiredLength = 12;
                options.Password.RequiredUniqueChars = 4;
                options.User.RequireUniqueEmail = true;
                options.SignIn.RequireConfirmedAccount = false;
                options.Lockout.AllowedForNewUsers = true;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
                options.Lockout.MaxFailedAccessAttempts = 5;
            })
            .AddEntityFrameworkStores<PortalClientDbContext>()
            .AddDefaultTokenProviders();

        services.Configure<DataProtectionTokenProviderOptions>(options =>
        {
            options.TokenLifespan = TimeSpan.FromMinutes(Math.Max(5, passwordResetOptions.TokenLifespanMinutes));
        });

        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.FromMinutes(5);
        });

        services.ConfigureApplicationCookie(options =>
        {
            options.Cookie.Name = "Simplix.PortalClient.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.SameSite = SameSiteMode.Lax;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.LoginPath = "/account/login";
            options.AccessDeniedPath = "/account/access-denied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
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

        services.AddHttpContextAccessor();
        services.AddScoped<IPortalExperienceService, PortalExperienceService>();
        services.AddScoped<IClientWorkspaceService, ClientWorkspaceService>();
        services.AddScoped<IClientSupportTicketService, ClientSupportTicketService>();
        services.AddScoped<IAdminWorkspaceService, AdminWorkspaceService>();
        services.AddScoped<ISecurityAuditService, SecurityAuditService>();
        services.AddHostedService<PortalClientDbContextInitializer>();

        return services;
    }
}

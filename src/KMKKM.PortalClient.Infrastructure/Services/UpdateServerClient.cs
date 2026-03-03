using System.Net.Http.Json;
using System.Reflection;
using KMKKM.PortalClient.Application.Contracts.Admin;
using KMKKM.PortalClient.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class UpdateServerClient
{
    private readonly HttpClient _httpClient;
    private readonly UpdateServerOptions _options;
    private readonly ILogger<UpdateServerClient> _logger;

    public UpdateServerClient(
        HttpClient httpClient,
        IOptions<UpdateServerOptions> options,
        ILogger<UpdateServerClient> logger)
    {
        _httpClient = httpClient;
        _options = options.Value;
        _logger = logger;
    }

    public async Task<SystemUpdateStatusViewModel> GetStatusAsync(CancellationToken cancellationToken)
    {
        if (!_options.EnableAutoUpdatePrompt || string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            return new SystemUpdateStatusViewModel(
                false,
                false,
                false,
                GetCurrentVersion(),
                GetCurrentVersion(),
                _options.CurrentChannel,
                "Verificacao automatica de atualizacao desabilitada.");
        }

        try
        {
            var response = await _httpClient.GetFromJsonAsync<UpdateVersionResponse>("api/version", cancellationToken);
            if (response is null)
            {
                return BuildUnavailable("Update Server sem resposta valida.");
            }

            return new SystemUpdateStatusViewModel(
                true,
                response.HasUpdate,
                response.RequiresAdminApproval,
                response.CurrentVersion ?? GetCurrentVersion(),
                response.TargetVersion ?? response.CurrentVersion ?? GetCurrentVersion(),
                response.Channel ?? _options.CurrentChannel,
                response.HasUpdate
                    ? $"Nova versao {response.TargetVersion} detectada. Aguardando aprovacao do Admin."
                    : $"Sistema atualizado na versao {response.CurrentVersion}.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to query Update Server.");
            return BuildUnavailable("Nao foi possivel consultar o Update Server.");
        }
    }

    private SystemUpdateStatusViewModel BuildUnavailable(string summary)
    {
        string currentVersion = GetCurrentVersion();
        return new SystemUpdateStatusViewModel(
            false,
            false,
            false,
            currentVersion,
            currentVersion,
            _options.CurrentChannel,
            summary);
    }

    private static string GetCurrentVersion()
    {
        return Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion
            ?? Assembly.GetEntryAssembly()?.GetName().Version?.ToString()
            ?? "0.0.0-local";
    }

    private sealed record UpdateVersionResponse(
        string? CurrentVersion,
        string? TargetVersion,
        string? Channel,
        bool HasUpdate,
        bool RequiresAdminApproval);
}

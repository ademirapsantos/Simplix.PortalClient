using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection;
using Simplix.PortalClient.Application.Contracts.Admin;
using Simplix.PortalClient.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Simplix.PortalClient.Infrastructure.Services;

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
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/version");
            AddAuthHeader(request);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                return BuildUnavailable("Update Server indisponivel.");
            }

            var payload = await response.Content.ReadFromJsonAsync<UpdateVersionResponse>(cancellationToken: cancellationToken);
            if (payload is null)
            {
                return BuildUnavailable("Update Server sem resposta valida.");
            }

            return new SystemUpdateStatusViewModel(
                true,
                payload.HasUpdate,
                payload.RequiresAdminApproval,
                payload.CurrentVersion ?? GetCurrentVersion(),
                payload.TargetVersion ?? payload.CurrentVersion ?? GetCurrentVersion(),
                payload.Channel ?? _options.CurrentChannel,
                payload.HasUpdate
                    ? $"Nova versao {payload.TargetVersion} detectada. Aguardando aprovacao do Admin."
                    : $"Sistema atualizado na versao {payload.CurrentVersion}.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to query Update Server.");
            return BuildUnavailable("Nao foi possivel consultar o Update Server.");
        }
    }

    public async Task<SystemUpdateTriggerResultViewModel> StartUpdateAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            return new SystemUpdateTriggerResultViewModel(false, false, "Update Server nao configurado.");
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Post, "api/update/start")
            {
                Content = JsonContent.Create(new { approvedBy = "admin" })
            };
            AddAuthHeader(request);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
                return new SystemUpdateTriggerResultViewModel(false, false, $"Falha ao iniciar update: {responseBody}");
            }

            var payload = await response.Content.ReadFromJsonAsync<UpdateStartResponse>(cancellationToken: cancellationToken);
            return new SystemUpdateTriggerResultViewModel(
                true,
                payload?.Started ?? true,
                payload?.Message ?? "Update iniciado com sucesso.");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to start system update.");
            return new SystemUpdateTriggerResultViewModel(false, false, "Nao foi possivel iniciar o update.");
        }
    }

    public async Task<SystemUpdateRuntimeStatusViewModel> GetRuntimeStatusAsync(CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            return new SystemUpdateRuntimeStatusViewModel(false, "indisponivel", "Update Server nao configurado.", null, null, null);
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "api/update/status");
            AddAuthHeader(request);

            using var response = await _httpClient.SendAsync(request, cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                string details = await response.Content.ReadAsStringAsync(cancellationToken);
                return new SystemUpdateRuntimeStatusViewModel(false, "erro", details, null, null, null);
            }

            var payload = await response.Content.ReadFromJsonAsync<UpdateRuntimeResponse>(cancellationToken: cancellationToken);
            return new SystemUpdateRuntimeStatusViewModel(
                payload?.InProgress ?? false,
                payload?.Stage ?? "desconhecido",
                payload?.LastError,
                payload?.TargetVersion,
                payload?.StartedAtUtc,
                payload?.FinishedAtUtc);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Unable to query runtime update status.");
            return new SystemUpdateRuntimeStatusViewModel(false, "erro", "Nao foi possivel consultar o status do update.", null, null, null);
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

    private void AddAuthHeader(HttpRequestMessage request)
    {
        if (!string.IsNullOrWhiteSpace(_options.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _options.Token);
        }
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

    private sealed record UpdateStartResponse(bool Started, string? Message);

    private sealed record UpdateRuntimeResponse(
        bool InProgress,
        string? Stage,
        string? LastError,
        string? TargetVersion,
        DateTimeOffset? StartedAtUtc,
        DateTimeOffset? FinishedAtUtc);
}

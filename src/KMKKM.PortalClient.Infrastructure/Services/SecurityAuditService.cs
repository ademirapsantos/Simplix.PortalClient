using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace KMKKM.PortalClient.Infrastructure.Services;

internal sealed class SecurityAuditService : ISecurityAuditService
{
    private readonly ILogger<SecurityAuditService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SecurityAuditService(
        ILogger<SecurityAuditService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public void Write(
        string eventType,
        bool succeeded,
        string? subject = null,
        Guid? userId = null,
        string? details = null)
    {
        HttpContext? httpContext = _httpContextAccessor.HttpContext;
        LogLevel logLevel = succeeded ? LogLevel.Information : LogLevel.Warning;

        _logger.Log(
            logLevel,
            "SecurityAudit event={EventType} succeeded={Succeeded} subject={Subject} userId={UserId} ip={IpAddress} userAgent={UserAgent} traceId={TraceId} details={Details}",
            eventType,
            succeeded,
            subject,
            userId,
            httpContext?.Connection.RemoteIpAddress?.ToString(),
            httpContext?.Request.Headers.UserAgent.ToString(),
            httpContext?.TraceIdentifier,
            details);
    }
}

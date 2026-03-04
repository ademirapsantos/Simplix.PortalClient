namespace Simplix.PortalClient.Infrastructure.Services;

public interface ISecurityAuditService
{
    void Write(
        string eventType,
        bool succeeded,
        string? subject = null,
        Guid? userId = null,
        string? details = null);
}


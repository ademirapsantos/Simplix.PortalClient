using Microsoft.AspNetCore.Identity;

namespace KMKKM.PortalClient.Infrastructure.Identity;

public sealed class ApplicationUser : IdentityUser<Guid>
{
    public string FullName { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

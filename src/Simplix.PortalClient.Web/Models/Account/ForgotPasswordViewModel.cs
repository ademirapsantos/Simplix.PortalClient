using System.ComponentModel.DataAnnotations;

namespace Simplix.PortalClient.Web.Models.Account;

public sealed class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
    public string Email { get; set; } = string.Empty;
}


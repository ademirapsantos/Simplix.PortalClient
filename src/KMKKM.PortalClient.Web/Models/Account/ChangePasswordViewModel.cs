using System.ComponentModel.DataAnnotations;

namespace KMKKM.PortalClient.Web.Models.Account;

public sealed class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Informe a senha atual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Senha atual")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nova senha")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme a nova senha.")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar nova senha")]
    [Compare(nameof(NewPassword), ErrorMessage = "A confirmacao da senha nao confere.")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

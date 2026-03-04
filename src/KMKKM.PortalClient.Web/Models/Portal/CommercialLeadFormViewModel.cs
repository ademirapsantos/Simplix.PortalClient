using System.ComponentModel.DataAnnotations;

namespace KMKKM.PortalClient.Web.Models.Portal;

public sealed class CommercialLeadFormViewModel
{
    [Required(ErrorMessage = "Informe seu nome.")]
    [StringLength(150, ErrorMessage = "Use ate 150 caracteres.")]
    [Display(Name = "Nome e sobrenome")]
    public string ContactName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe a empresa.")]
    [StringLength(150, ErrorMessage = "Use ate 150 caracteres.")]
    [Display(Name = "Empresa")]
    public string CompanyName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o e-mail.")]
    [EmailAddress(ErrorMessage = "Informe um e-mail valido.")]
    [Display(Name = "E-mail")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe um telefone.")]
    [StringLength(40, ErrorMessage = "Use ate 40 caracteres.")]
    [Display(Name = "Telefone")]
    public string Phone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Informe o produto de interesse.")]
    [StringLength(150, ErrorMessage = "Use ate 150 caracteres.")]
    [Display(Name = "Produto de interesse")]
    public string InterestedProduct { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descreva o que voce precisa.")]
    [StringLength(2000, ErrorMessage = "Use ate 2000 caracteres.")]
    [Display(Name = "Mensagem")]
    public string Message { get; set; } = string.Empty;
}

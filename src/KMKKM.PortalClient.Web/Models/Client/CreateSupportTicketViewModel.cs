using KMKKM.PortalClient.Domain.Constants;
using System.ComponentModel.DataAnnotations;

namespace KMKKM.PortalClient.Web.Models.Client;

public sealed class CreateSupportTicketViewModel
{
    [Required(ErrorMessage = "Informe o titulo do chamado.")]
    [StringLength(200, ErrorMessage = "Use ate 200 caracteres.")]
    [Display(Name = "Titulo")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descreva o chamado.")]
    [StringLength(4000, ErrorMessage = "Use ate 4000 caracteres.")]
    [Display(Name = "Descricao")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecione o tipo do chamado.")]
    [Display(Name = "Tipo")]
    public string Category { get; set; } = SupportTicketCategories.Bug;

    [Required(ErrorMessage = "Selecione a prioridade.")]
    [Display(Name = "Prioridade")]
    public string Priority { get; set; } = SupportTicketPriorities.Medium;
}

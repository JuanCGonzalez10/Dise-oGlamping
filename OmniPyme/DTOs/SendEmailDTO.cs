using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class SendEmailDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "El campo {0} no es una dirección de correo válida.")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Subject { get; set; } = null!;

        [Required(ErrorMessage = "El campo {0} es requerido.")]
        public string Content { get; set; } = null!;
    }
}

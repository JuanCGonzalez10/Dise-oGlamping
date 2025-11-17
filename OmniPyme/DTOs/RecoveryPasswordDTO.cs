using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class RecoveryPasswordDTO
    {
        [Required(ErrorMessage = "El campo {0} es requerido.")]
        [EmailAddress(ErrorMessage = "El campo {0} no es una dirección de correo válida.")]
        public string Email { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class ChangePasswordDTO
    {
        [Display(Name = "Contraseña actual")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MinLength(4, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "Nueva contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MinLength(4, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Display(Name = "Confirmación de contraseña")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [MinLength(4, ErrorMessage = "El campo {0} debe tener una longitud mínima de {1} carácteres.")]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
        public string ConfirmPassword { get; set; }
    }
}

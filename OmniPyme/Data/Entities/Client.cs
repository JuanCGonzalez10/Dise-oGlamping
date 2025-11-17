using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OmniPyme.Web.Data.Entities
{
    public class Client : IId
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Cedula")]
        public string DNI { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombres")]
        public string FirstName { get; set; } = null!;


        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Apellidos")]
        public string LastName { get; set; } = null!;

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [EmailAddress(ErrorMessage = "El campo {0} debe ser un correo electrónico válido")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Telefono")]
        public string Phone { get; set; } = null!;

        [Display(Name = "Fecha de Registro")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime RegisterDate { get; set; }

        [Display(Name = "Fecha de ultima compra")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime LastPurchaseDate { get; set; }
    }
}

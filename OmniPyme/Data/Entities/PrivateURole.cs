using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.Data.Entities
{
    public class PrivateURole
    {
        [Key]

        public int Id { get; set; }


        [Display(Name = "Nombres")]
        [MaxLength(45, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; } = null!;

        public ICollection<RolePermission>? RolePermissions { get; set; }

    }
}

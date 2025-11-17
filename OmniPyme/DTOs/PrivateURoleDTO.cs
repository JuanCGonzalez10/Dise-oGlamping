using OmniPyme.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class PrivateURoleDTO
    {
        public int Id { get; set; }


        [Display(Name = "Nombres")]
        [MaxLength(45, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string Name { get; set; } = null!;

        public List<PermissionForRoleDTO>? Permissions { get; set; }
        public string? PermissionIds { get; set; }

        public class PermissionForRoleDTO : PermissionDTO
        {
            public bool Selected { get; set; }
        }
    }
}

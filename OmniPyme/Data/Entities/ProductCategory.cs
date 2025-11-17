using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace OmniPyme.Web.Data.Entities
{
    public class ProductCategory : IId
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Categoria Del Producto")]
        public string ProductCategoryName { get; set; } = null!;
    }
}
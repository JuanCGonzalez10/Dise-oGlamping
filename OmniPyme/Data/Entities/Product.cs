using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OmniPyme.Web.Data.Entities
{
    public class Product : IId
    {
        [Key]
        public int Id { get; set; }


        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre Del Producto")]
        public string ProductName { get; set; } = null!;


        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Descripcion")]
        public string ProductDescription { get; set; } = null!;

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Precio")]
        [Precision(18, 2)]
        public decimal ProductPrice { get; set; }

        [MaxLength(45, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Codido de Barras")]
        public string ProductBarCode { get; set; } = null!;

        [Display(Name = "Impuestos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public double ProductTax { get; set; }

        [ForeignKey("ProductCategory")] // Indica que IdProductCategory es la clave foránea
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdProductCategory { get; set; }
        public ProductCategory? ProductCategory { get; set; }

    }
}

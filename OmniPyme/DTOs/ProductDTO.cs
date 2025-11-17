using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.DTOs
{
    public class ProductDTO : IId
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

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Precio")]
        [Range(0, (double)decimal.MaxValue, ErrorMessage = "El campo {0} debe ser un valor positivo")]
        public decimal? ProductPrice { get; set; }

        [MaxLength(45, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Codido de Barras")]
        public string ProductBarCode { get; set; } = null!;

        [Display(Name = "Impuestos")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0.01, 100.00, ErrorMessage = "El campo {0} debe estar entre 0 y 100")]
        public double? ProductTax { get; set; }


        public ProductCategory? ProductCategory { get; set; }
        [Display(Name = "Categoria de Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdProductCategory { get; set; }

        public IEnumerable<SelectListItem>? ProductCategories { get; set; }
    }
}

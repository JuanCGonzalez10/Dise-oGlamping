using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OmniPyme.Web.Data.Entities
{
    public class SaleDetail : IId
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Codigo de Venta")]
        public string SaleDetailCode { get; set; }
        [Display(Name = "Cantidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int SaleDetailProductQuantity { get; set; }
        [Display(Name = "Precio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Precision(18, 2)]
        public decimal SaleDetailProductPrice { get; set; }
        [Display(Name = "Impuesto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Precision(18, 2)]
        public decimal SaleDetailProductTax { get; set; }
        [Display(Name = "Subtotal")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Precision(18, 2)]
        public decimal SaleDetailSubtotal { get; set; }
        [Display(Name = "Codigo de Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [ForeignKey("Product")] // Indica que IdSale es la clave foránea
        public int SaleDetailProductCode { get; set; }
        public Product? Product { get; set; }  // Relación con la entidad Product

        [ForeignKey("Sale")] // Indica que IdSale es la clave foránea
        public int IdSale { get; set; }
        public Sale? Sale { get; set; }  // Relación con la entidad Sale

    }
}

using System.ComponentModel.DataAnnotations;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.DTOs
{
    public class SaleDetailDTO
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
        public decimal SaleDetailProductPrice { get; set; }
        [Display(Name = "Subtotal")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal SaleDetailSubtotal { get; set; }
        [Display(Name = "Impuesto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public decimal SaleDetailProductTax { get; set; }
        [Display(Name = "Codigo de Producto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int SaleDetailProductCode { get; set; }
        public Product? Product { get; set; }
        public Sale? Sale { get; set; }
        public int IdSale { get; set; }
    }
}

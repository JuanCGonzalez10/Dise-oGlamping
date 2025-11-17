using Microsoft.EntityFrameworkCore;
using OmniPyme.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class SaleDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha de venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime SaleDate { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Metodo de pago")]
        public string SalePaymentMethod { get; set; } = null!;
        [Display(Name = "Impuesto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public double SaleDetailProductTax { get; set; }
        [Display(Name = "Total")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Precision(18, 2)]
        public decimal SaleTotal { get; set; }

        [Display(Name = "Codigo de Venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string SaleCode { get; set; }

        public Client? Client { get; set; }

        public int IdClient { get; set; }

        //[Display(Name = "Vendedor")]
        //[Required(ErrorMessage = "El campo {0} es obligatorio")]
        //public User User { get; set; }

        //public int IdUser { get; set; }
    }
}

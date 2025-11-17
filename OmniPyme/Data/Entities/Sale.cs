using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace OmniPyme.Web.Data.Entities
{
    public class Sale : IId
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

        [Display(Name = "Total")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Precision(18, 2)]
        public decimal SaleTotal { get; set; }

        [Display(Name = "Codigo de Venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string SaleCode { get; set; }

        [ForeignKey("Client")] // Indica que IdClient es la clave foránea
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdClient { get; set; }
        public Client? Client { get; set; }  // Relación con la entidad Client
    }
}

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniPyme.Web.Data.Entities
{
    public class Invoice : IId
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha de venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime InvoiceDate { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Metodo de pago")]
        public string InvoiceNumber { get; set; } = null!;

        [ForeignKey("Sale")] // Indica que IdSale es la clave foránea
        public int IdSale { get; set; }
        public Sale? Sale { get; set; }

    }
}

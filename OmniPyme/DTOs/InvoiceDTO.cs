using Microsoft.AspNetCore.Mvc.Rendering;
using OmniPyme.Web.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace OmniPyme.Web.DTOs
{
    public class InvoiceDTO
    {
        [Key]
        public int Id { get; set; }

        [Display(Name = "Fecha de venta")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public DateTime InvoiceDate { get; set; }

        [MaxLength(32, ErrorMessage = "El campo {0} debe tener máximo {1} caracteres")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Numero de Factura")]
        public string InvoiceNumber { get; set; } = null!;

        public Sale? Sale { get; set; }

        public int IdSale { get; set; }

        public Client? Client { get; set; }

        [Display(Name = "Cliente")]
        [Range(1, int.MaxValue, ErrorMessage = "Debe Seleccionar un cliente")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int IdClient { get; set; }

        public IEnumerable<SelectListItem>? Clients { get; set; } = null!;
    }
}

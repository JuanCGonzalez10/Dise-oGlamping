using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.DTOs
{
    public class ReservationDTO : IId
    {
        [Key]
        public int Id { get; set; }

        // =====================================================
        // PRODUCTO (ALOJAMIENTO)
        // =====================================================

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Alojamiento / Glamping")]
        public int ProductId { get; set; }

        public Product? Product { get; set; }

        public IEnumerable<SelectListItem>? Products { get; set; }


        // =====================================================
        // USUARIO (CLIENTE)
        // =====================================================

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Cliente / Usuario")]
        public string UserId { get; set; } = null!;

        public Users? User { get; set; }

        public IEnumerable<SelectListItem>? Users { get; set; }


        // =====================================================
        // FECHAS DE LA RESERVA
        // =====================================================

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-In")]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [DataType(DataType.Date)]
        [Display(Name = "Check-Out")]
        public DateTime CheckOut { get; set; }


        // =====================================================
        // CALCULADOS
        // =====================================================

        [Display(Name = "Noches")]
        public int Nights { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Precio por Noche")]
        public decimal PricePerNight { get; set; }

        [Precision(18, 2)]
        [Display(Name = "Total a Pagar")]
        public decimal Total { get; set; }


        // =====================================================
        // PAGO
        // =====================================================

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Método de Pago")]
        public string PaymentMethod { get; set; } = null!;

        [Display(Name = "Instrucciones de Pago")]
        [MaxLength(500)]
        public string? PaymentInstructions { get; set; }


        // =====================================================
        // ESTADO
        // =====================================================

        [Required]
        [Display(Name = "Estado")]
        public string Status { get; set; } = "PendientePago";


        // =====================================================
        // METADATA
        // =====================================================

        [Display(Name = "Creado")]
        public DateTime CreatedAt { get; set; }
    }
}

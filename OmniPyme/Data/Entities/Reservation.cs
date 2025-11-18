using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace OmniPyme.Web.Data.Entities
{
    public class Reservation: IId 
    {
        [Key]
        public int Id { get; set; }

        // ------------------------
        // RELACIÓN CON PRODUCT
        // ------------------------
        [Required]
        [Display(Name = "Alojamiento / Glamping")]
        public int ProductId { get; set; }
        public Product Product { get; set; }

        // ------------------------
        // RELACIÓN CON USERS
        // ------------------------
        [Required]
        public string UserId { get; set; }
        public Users User { get; set; }

        // ------------------------
        // DATOS DE LA RESERVA
        // ------------------------
        [Required]
        [Display(Name = "Check-In")]
        public DateTime CheckIn { get; set; }

        [Required]
        [Display(Name = "Check-Out")]
        public DateTime CheckOut { get; set; }

        public int Nights { get; set; }

        [Precision(18, 2)]
        public decimal PricePerNight { get; set; }

        [Precision(18, 2)]
        public decimal Total { get; set; }

        [Required]
        [Display(Name = "Método de Pago")]
        public string PaymentMethod { get; set; }

        public string? PaymentInstructions { get; set; }

        [Required]
        [Display(Name = "Estado")]
        public string Status { get; set; } = "PendientePago";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}

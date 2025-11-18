using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Services
{
    public class ReservationsService : CustomQueryableOperations, IReservationsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ReservationsService(DataContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================================================
        // CREATE (con reglas del punto 6 completas)
        // ============================================================
        public async Task<Response<ReservationDTO>> CreateAsync(ReservationDTO dto)
        {
            // Validar que el producto exista
            Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == dto.ProductId);
            if (product == null)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("El glamping seleccionado no existe.");

            // Validar usuario
            bool userExists = await _context.Users.AnyAsync(x => x.Id == dto.UserId);
            if (!userExists)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("El usuario seleccionado no existe.");

            // Convertir fechas a UTC (regla #2)
            DateTime checkInUtc = dto.CheckIn.ToUniversalTime();
            DateTime checkOutUtc = dto.CheckOut.ToUniversalTime();

            // Validar rango de fechas
            if (checkOutUtc <= checkInUtc)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("La fecha de salida debe ser mayor a la de entrada.");

            // Regla #3 — evitar traslapes/overbooking
            bool overlapping = await _context.Reservations.AnyAsync(r =>
                 r.ProductId == dto.ProductId &&
                 r.CheckIn < checkOutUtc &&
                 checkInUtc < r.CheckOut);

            if (overlapping)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("Ya existe una reserva en esas fechas.");

            // Recalcular noches y total en servidor (regla #1)
            dto.Nights = (int)(checkOutUtc - checkInUtc).TotalDays;
            if (dto.Nights <= 0)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("La reserva debe ser al menos de una noche.");

            dto.PricePerNight = product.ProductPrice;
            dto.Total = dto.PricePerNight * dto.Nights;

            // Guardar fechas realmente en UTC
            dto.CheckIn = checkInUtc;
            dto.CheckOut = checkOutUtc;

            // // ELIMINAR O COMENTAR ESTA SECCIÓN:
            // dto.PaymentInstructions =
            //     $"Consignar {dto.Total:C} al banco XXX. Referencia: {Guid.NewGuid()}";

            dto.CreatedAt = DateTime.UtcNow;
            dto.Status = dto.Status ?? "PendientePago";

            try
            {
                // 1. Mapear DTO a Entity. AutoMapper ignorará las colecciones (SelectListItem)
                Reservation entity = _mapper.Map<Reservation>(dto);

                // 2. CRÍTICO: Asegurarse de que las propiedades de navegación (Product y User) 
                // sean NULAS para que Entity Framework solo use las Foreign Keys (ProductId, UserId).
                // Esto previene un error si AutoMapper intentó adjuntar un objeto Product/User incompleto.
                entity.Product = null;
                entity.User = null;

                // 3. Agregar y guardar la entidad
                _context.Reservations.Add(entity);
                await _context.SaveChangesAsync();

                // 4. Mapear de vuelta si necesita el ID generado u otros campos
                ReservationDTO resultDto = _mapper.Map<ReservationDTO>(entity);

                return ResponseHelper<ReservationDTO>.MakeResponseSuccess(resultDto, "Reserva creada correctamente.");
            }
            catch (Exception ex)
            {
                // Esto capturará la excepción real de la DB si ocurre.
                // El mensaje de error será más útil internamente, pero al usuario se le mostrará el mensaje genérico de error.

                // Si necesitas ver el error exacto para debuggear, descomenta esta línea:
                // System.Diagnostics.Debug.WriteLine($"DB SAVE ERROR: {ex.Message} - Inner: {ex.InnerException?.Message}");

                return ResponseHelper<ReservationDTO>.MakeResponseFail("Ha ocurrido un error al guardar la reserva. (DB Error)");
            }
        }

        // ============================================================
        // EDIT (con recalculo y validación de fechas)
        // ============================================================
        public async Task<Response<ReservationDTO>> EditAsync(ReservationDTO dto)
        {
            bool exists = await _context.Reservations.AnyAsync(x => x.Id == dto.Id);
            if (!exists)
                return ResponseHelper<ReservationDTO>.MakeResponseFail($"No existe la reserva con id {dto.Id}");

            // Obtener el producto
            Product? product = await _context.Products.FirstAsync(x => x.Id == dto.ProductId);

            // Convertir fechas a UTC
            DateTime checkInUtc = dto.CheckIn.ToUniversalTime();
            DateTime checkOutUtc = dto.CheckOut.ToUniversalTime();

            if (checkOutUtc <= checkInUtc)
                return ResponseHelper<ReservationDTO>.MakeResponseFail("La fecha de salida debe ser mayor a la de entrada.");

            dto.CheckIn = checkInUtc;
            dto.CheckOut = checkOutUtc;

            // Recalcular noches y total
            dto.Nights = (int)(checkOutUtc - checkInUtc).TotalDays;
            dto.PricePerNight = product.ProductPrice;
            dto.Total = dto.PricePerNight * dto.Nights;

            return await EditAsync<Reservation, ReservationDTO>(dto, dto.Id);
        }

        // ============================================================
        // DELETE (con política de cancelación de 24h)
        // ============================================================
        public async Task<Response<object>> DeleteAsync(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
                return ResponseHelper<object>.MakeResponseFail($"La reserva con id {id} no existe");

            // Regla #2 — No cancelar si faltan menos de 24h para check-in
            double horasRestantes = (reservation.CheckIn - DateTime.UtcNow).TotalHours;
            if (horasRestantes < 24)
            {
                return ResponseHelper<object>.MakeResponseFail(
                    "No es posible cancelar una reserva con menos de 24 horas antes del check-in."
                );
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return ResponseHelper<object>.MakeResponseSuccess(null, "Reserva cancelada correctamente.");
        }

        // ============================================================
        // GET ONE
        // ============================================================
        public async Task<Response<ReservationDTO>> GetOneAsync(int id)
        {
            try
            {
                Reservation? entity = await _context.Reservations
                    .Include(r => r.Product)
                    .Include(r => r.User)
                    .FirstOrDefaultAsync(x => x.Id == id);

                if (entity is null)
                    return ResponseHelper<ReservationDTO>.MakeResponseFail($"No existe la reserva con id {id}");

                ReservationDTO dto = _mapper.Map<ReservationDTO>(entity);
                return ResponseHelper<ReservationDTO>.MakeResponseSuccess(dto, "Reserva encontrada con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<ReservationDTO>.MakeResponseFail(ex);
            }
        }

        // ============================================================
        // PAGINACIÓN
        // ============================================================
        public async Task<Response<PaginationResponse<ReservationDTO>>> GetPaginationAsync(PaginationRequest request)
        {
            IQueryable<Reservation> query = _context.Reservations
                .Include(r => r.Product)
                .Include(r => r.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(r =>
                    r.Product!.ProductName.Contains(request.Filter) ||
                    r.User!.FirstName.Contains(request.Filter) ||
                    r.User!.LastName.Contains(request.Filter));
            }

            query = query.OrderBy(r => r.Id);

            return await GetPaginationAsync<Reservation, ReservationDTO>(request, query);
        }

        // ============================================================
        // LISTA (PARA COMBOS)
        // ============================================================
        public async Task<List<ReservationDTO>> GetReservationListAsync()
        {
            return await _context.Reservations
                .Select(r => new ReservationDTO
                {
                    Id = r.Id,
                    ProductId = r.ProductId,
                    UserId = r.UserId,
                    Nights = r.Nights,
                    Total = r.Total,
                    CheckIn = r.CheckIn,
                    CheckOut = r.CheckOut,
                }).ToListAsync();
        }
        // ============================================================
        // PRECIOS CRÍTICOS
        // ============================================================

        public async Task<Dictionary<int, decimal>> GetProductPricesMapAsync()
        {
            // Obtiene un diccionario con ProductId como clave y ProductPrice como valor
            return await _context.Products
                .ToDictionaryAsync(p => p.Id, p => p.ProductPrice);
        }

        public async Task<decimal?> GetProductPriceAsync(int productId)
        {
            // Obtiene el precio de un solo producto.
            decimal? price = await _context.Products
                .Where(p => p.Id == productId)
                .Select(p => (decimal?)p.ProductPrice)
                .FirstOrDefaultAsync();

            return price;
        }



    }
}



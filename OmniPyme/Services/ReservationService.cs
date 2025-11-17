using AutoMapper;
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
        // CREATE
        // ============================================================
        public async Task<Response<ReservationDTO>> CreateAsync(ReservationDTO dto)
        {
            // validar que el producto exista
            bool productExists = await _context.Products.AnyAsync(x => x.Id == dto.ProductId);
            if (!productExists)
            {
                return ResponseHelper<ReservationDTO>.MakeResponseFail("El producto seleccionado no existe.");
            }

            // validar usuario
            bool userExists = await _context.Users.AnyAsync(x => x.Id == dto.UserId);
            if (!userExists)
            {
                return ResponseHelper<ReservationDTO>.MakeResponseFail("El usuario seleccionado no existe.");
            }

            // validar fechas
            if (dto.CheckOut <= dto.CheckIn)
            {
                return ResponseHelper<ReservationDTO>.MakeResponseFail("La fecha de salida debe ser mayor a la de entrada.");
            }

            // calcular noches y totales
            Product? product = await _context.Products.FirstAsync(x => x.Id == dto.ProductId);
            dto.PricePerNight = product.ProductPrice;
            dto.Nights = (dto.CheckOut - dto.CheckIn).Days;
            dto.Total = dto.PricePerNight * dto.Nights;
            dto.CreatedAt = DateTime.UtcNow;

            return await CreateAsync<Reservation, ReservationDTO>(dto);
        }

        // ============================================================
        // EDIT
        // ============================================================
        public async Task<Response<ReservationDTO>> EditAsync(ReservationDTO dto)
        {
            bool exists = await _context.Reservations.AnyAsync(x => x.Id == dto.Id);
            if (!exists)
            {
                return ResponseHelper<ReservationDTO>.MakeResponseFail($"No existe la reserva con id {dto.Id}");
            }

            // recalcular totales
            Product? product = await _context.Products.FirstAsync(x => x.Id == dto.ProductId);
            dto.PricePerNight = product.ProductPrice;
            dto.Nights = (dto.CheckOut - dto.CheckIn).Days;
            dto.Total = dto.PricePerNight * dto.Nights;

            return await EditAsync<Reservation, ReservationDTO>(dto, dto.Id);
        }

        // ============================================================
        // DELETE
        // ============================================================
        public async Task<Response<object>> DeleteAsync(int id)
        {
            var response = await DeleteAsync<Reservation>(id);
            response.Message = !response.IsSuccess ? $"La reserva con id {id} no existe" : response.Message;
            return response;
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
                {
                    return ResponseHelper<ReservationDTO>.MakeResponseFail($"No existe la reserva con id {id}");
                }

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
    }
}


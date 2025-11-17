using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;

namespace OmniPyme.Web.Services
{
    public interface IReservationsService
    {
        Task<Response<ReservationDTO>> CreateAsync(ReservationDTO dto);
        Task<Response<ReservationDTO>> EditAsync(ReservationDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<ReservationDTO>> GetOneAsync(int id);
        Task<Response<PaginationResponse<ReservationDTO>>> GetPaginationAsync(PaginationRequest request);

        // listas para combos
        Task<List<ReservationDTO>> GetReservationListAsync();
    }
}


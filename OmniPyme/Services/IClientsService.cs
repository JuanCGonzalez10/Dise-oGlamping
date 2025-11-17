using System.Linq;
using AutoMapper;
using Humanizer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Services
{
    public interface IClientsService
    {
        public Task<Response<ClientDTO>> CreateAsync(ClientDTO dto);
        public Task<Response<object>> DeleteAsync(int id);
        public Task<Response<ClientDTO>> EditAsync(ClientDTO dto);
        public Task<Response<List<ClientDTO>>> GetListAsync();
        public Task<Response<ClientDTO>> GetOneAsync(int id);
        public Task<Response<PaginationResponse<ClientDTO>>> GetPaginationAsync(PaginationRequest request);
    }

    public class ClientsService : CustomQueryableOperations, IClientsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ClientsService(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<ClientDTO>> CreateAsync(ClientDTO dto)
        {
            return await CreateAsync<Client, ClientDTO>(dto);
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            Response<object> response = await DeleteAsync<Client>(id);
            response.Message = !response.IsSuccess ? $"El cliente con id: {id} no existe" : response.Message;
            return response;
        }

        public async Task<Response<ClientDTO>> EditAsync(ClientDTO dto)
        {
            return await EditAsync<Client, ClientDTO>(dto, dto.Id);
        }

        public async Task<Response<List<ClientDTO>>> GetListAsync()
        {
            try
            {
                List<Client> clients = await _context.Clients.ToListAsync();

                List<ClientDTO> list = _mapper.Map<List<ClientDTO>>(clients);

                return ResponseHelper<List<ClientDTO>>.MakeResponseSuccess(list);
            }
            catch (Exception ex)
            {
                return ResponseHelper<List<ClientDTO>>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<ClientDTO>> GetOneAsync(int id)
        {
            return await GetOneAsync<Client, ClientDTO>(id);
        }

        public async Task<Response<PaginationResponse<ClientDTO>>> GetPaginationAsync(PaginationRequest request)
        {



            IQueryable<Client> query = _context.Clients.AsNoTracking().AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(c => c.DNI.ToLower().Contains(request.Filter.ToLower())
                                        || c.FirstName.ToLower().Contains(request.Filter.ToLower())
                                        || c.LastName.ToLower().Contains(request.Filter.ToLower())
                                        || (c.FirstName + " " + c.LastName).ToLower().Contains(request.Filter.ToLower())
                                        || (c.LastName + " " + c.FirstName).ToLower().Contains(request.Filter.ToLower()));
            }

            query = query.OrderBy(c => c.Id);

            return await GetPaginationAsync<Client, ClientDTO>(request, query);
        }


    }
}

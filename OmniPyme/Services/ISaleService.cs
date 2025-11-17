using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Services
{
    public interface ISalesService
    {
        public Task<Response<SaleDTO>> CreateAsync(SaleDTO dto);
        public Task<Response<object>> DeleteAsync(int id);
        public Task<Response<SaleDTO>> EditAsync(SaleDTO dto);
        public Task<Response<SaleDTO>> GetOneAsync(int id);
    }

    public class SalesService : CustomQueryableOperations, ISalesService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public SalesService(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<SaleDTO>> CreateAsync(SaleDTO dto)
        {
            // Obtener el último registro ordenado por SaleCode
            Sale? lastSale = await _context.Sales
                .OrderByDescending(s => s.Id)
                .FirstOrDefaultAsync();

            string nextCode = "V001";

            if (lastSale != null && lastSale.SaleCode != null && lastSale.SaleCode.Length > 1)
            {
                string numberPart = lastSale.SaleCode.Substring(1);
                bool isParsed = int.TryParse(numberPart, out int number);
                if (isParsed)
                {
                    nextCode = "V" + (number + 1).ToString("D3");
                }
            }

            // Asignar el código generado al DTO
            dto.SaleCode = nextCode;

            return await CreateAsync<Sale, SaleDTO>(dto);
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            Response<object> response = await DeleteAsync<Sale>(id);
            response.Message = !response.IsSuccess ? $"La factura con id: {id} no existe" : response.Message;
            return response;
        }

        public async Task<Response<SaleDTO>> EditAsync(SaleDTO dto)
        {
            return await EditAsync<Sale, SaleDTO>(dto, dto.Id);
        }

        public async Task<Response<SaleDTO>> GetOneAsync(int id)
        {
            try
            {
                Sale? entity = await _context.Sales
                    .Include(s => s.Client)
                    .FirstOrDefaultAsync(s => s.Id == id);

                if (entity is null)
                {
                    return ResponseHelper<SaleDTO>.MakeResponseFail($"No existe el registro con id {id}");
                }
                SaleDTO dto = _mapper.Map<SaleDTO>(entity);
                return ResponseHelper<SaleDTO>.MakeResponseSuccess(dto, "Registro encontrado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<SaleDTO>.MakeResponseFail(ex);
            }
        }
    }
}
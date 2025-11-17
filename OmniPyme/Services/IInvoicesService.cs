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
    public interface IInvoicesService
    {
        public Task<Response<InvoiceDTO>> CreateAsync(InvoiceDTO dto);
        public Task<Response<object>> DeleteAsync(int id);
        public Task<Response<InvoiceDTO>> EditAsync(InvoiceDTO dto);
        public Task<Response<InvoiceDTO>> GetOneAsync(int id);
        public Task<Response<PaginationResponse<InvoiceDTO>>> GetPaginationAsync(PaginationRequest request);
        public Task<String> GetNextInvoiceNumberAsync();
    }

    public class InvoicesService : CustomQueryableOperations, IInvoicesService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public InvoicesService(DataContext context, IMapper mapper) : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<InvoiceDTO>> CreateAsync(InvoiceDTO dto)
        {
            return await CreateAsync<Invoice, InvoiceDTO>(dto);
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            Response<object> response = await DeleteAsync<Invoice>(id);
            response.Message = !response.IsSuccess ? $"La factura con id: {id} no existe" : response.Message;
            return response;
        }

        public async Task<Response<InvoiceDTO>> EditAsync(InvoiceDTO dto)
        {
            return await EditAsync<Invoice, InvoiceDTO>(dto, dto.Id);
        }

        public async Task<Response<InvoiceDTO>> GetOneAsync(int id)
        {
            //return await GetOneAsync<Invoice, InvoiceDTO>(id); //No se puede usar porque este medoto no incluye la venta
            try
            {
                Invoice? entity = await _context.Invoices
                    .Include(i => i.Sale)
                    .ThenInclude(s => s.Client)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (entity is null)
                {
                    return ResponseHelper<InvoiceDTO>.MakeResponseFail($"No existe la factura con id {id}");
                }


                InvoiceDTO dto = _mapper.Map<InvoiceDTO>(entity);

                return ResponseHelper<InvoiceDTO>.MakeResponseSuccess(dto, "Factura cargada correctamente");
            }
            catch (Exception ex)
            {
                return ResponseHelper<InvoiceDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<PaginationResponse<InvoiceDTO>>> GetPaginationAsync(PaginationRequest request)
        {
            IQueryable<Invoice> query = _context.Invoices
                .Include(i => i.Sale)
                .ThenInclude(s => s.Client)
                .Select(i => new Invoice
                {
                    Id = i.Id,
                    InvoiceNumber = i.InvoiceNumber,
                    InvoiceDate = i.InvoiceDate,
                    Sale = new Sale
                    {
                        Id = i.Sale.Id,
                        SaleTotal = i.Sale.SaleTotal
                    }
                })
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Filter))
            {
                query = query.Where(i => i.InvoiceNumber.ToLower()
                                            .Contains(request.Filter
                                            .ToLower()));
            }

            query = query.OrderBy(i => i.InvoiceDate);

            return await GetPaginationAsync<Invoice, InvoiceDTO>(request, query);
        }

        public async Task<String> GetNextInvoiceNumberAsync()
        {
            try
            {
                // Buscar la última factura
                Invoice? lastInvoice = await _context.Invoices
                    .OrderByDescending(i => i.InvoiceNumber)
                    .FirstOrDefaultAsync();

                string year = DateTime.Now.Year.ToString();
                string nextNumber = "0001";

                if (lastInvoice != null && !string.IsNullOrEmpty(lastInvoice.InvoiceNumber))
                {
                    // Ejemplo: FAC-2025-0012
                    string[] parts = lastInvoice.InvoiceNumber.Split('-');
                    if (parts.Length == 3 && parts[1] == year)
                    {
                        string numberPart = parts[2];
                        if (int.TryParse(numberPart, out int currentNumber))
                        {
                            nextNumber = (currentNumber + 1).ToString("D4");
                        }
                    }
                }

                return $"FAC-{year}-{nextNumber}";
            }
            catch (Exception ex)
            {
                return ResponseHelper<string>.MakeResponseFail(ex).Message;
            }
        }
    }
}

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Services
{
    public interface IProductsService
    {
        Task<Response<ProductDTO>> CreateAsync(ProductDTO dto);
        Task<Response<ProductDTO>> EditAsync(ProductDTO dto);
        Task<Response<object>> DeleteAsync(int id);
        Task<Response<ProductDTO>> GetOneAsync(int id);
        Task<Response<PaginationResponse<ProductDTO>>> GetPaginationAsync(PaginationRequest request);
        Task<List<ProductDTO>> GetProductListAsync();
    }

    public class ProductsService : CustomQueryableOperations, IProductsService
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;

        public ProductsService(DataContext context, IMapper mapper)
            : base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Response<ProductDTO>> CreateAsync(ProductDTO dto)
        {
            // validar que el producto no exista
            bool exists = await _context.Products
                .AnyAsync(x => x.ProductName.ToLower() == dto.ProductName.Trim().ToLower());
            if (exists)
            {
                return ResponseHelper<ProductDTO>.MakeResponseFail("Ya existe un producto con ese nombre.");
            }
            return await CreateAsync<Product, ProductDTO>(dto);
        }

        public async Task<Response<ProductDTO>> EditAsync(ProductDTO dto)
        {
            // validar que el producto si exista
            bool exists = await _context.Products
                .AnyAsync(x => x.Id == dto.Id);
            if (!exists)
            {
                return ResponseHelper<ProductDTO>.MakeResponseFail($"No existe el producto con id {dto.Id}");
            }
            return await EditAsync<Product, ProductDTO>(dto, dto.Id);
        }

        public async Task<Response<object>> DeleteAsync(int id)
        {
            Response<object> response = await DeleteAsync<Product>(id);
            response.Message = !response.IsSuccess ? $"El producto con id: {id} no existe" : response.Message;
            return response;
        }

        public async Task<Response<ProductDTO>> GetOneAsync(int id)
        {
            try
            {
                Product? entity = await _context.Products
                    .Include(p => p.ProductCategory)
                    .FirstOrDefaultAsync(x => x.Id == id);
                if (entity is null)
                {
                    return ResponseHelper<ProductDTO>.MakeResponseFail($"No existe el producto con id {id}");
                }

                ProductDTO dto = _mapper.Map<ProductDTO>(entity);
                return ResponseHelper<ProductDTO>.MakeResponseSuccess(dto, "Producto encontrado con éxito");
            }
            catch (Exception ex)
            {
                return ResponseHelper<ProductDTO>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<PaginationResponse<ProductDTO>>> GetPaginationAsync(PaginationRequest request)
        {
            IQueryable<Product> query = _context.Products
                .Include(p => p.ProductCategory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(request.Filter))
            {
                query = query.Where(p =>
                    p.ProductName.Contains(request.Filter) ||
                    p.ProductBarCode.Contains(request.Filter) ||
                    p.ProductCategory!.ProductCategoryName.Contains(request.Filter));
            }

            query = query.OrderBy(p => p.Id);

            return await GetPaginationAsync<Product, ProductDTO>(request, query);
        }

        public async Task<List<ProductDTO>> GetProductListAsync()
        {
            return await _context.Products
                .Select(p => new ProductDTO
                {
                    Id = p.Id,
                    ProductName = p.ProductName,
                    ProductPrice = p.ProductPrice,
                    ProductTax = p.ProductTax
                }).ToListAsync();
        }
    }
}

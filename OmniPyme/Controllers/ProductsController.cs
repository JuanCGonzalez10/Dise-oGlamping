using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Helpers;
using AspNetCoreHero.ToastNotification.Abstractions;
using OmniPyme.Web.Services;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Core;
using OmniPyme.Web.DTOs;
using Microsoft.AspNetCore.Authorization;
using OmniPyme.Web.Core.Attributes;

namespace OmniPyme.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IProductsService _productsService;
        private readonly ICombosHelper _combosHelper;
        private readonly INotyfService _notyf;
        private readonly IUsersService _usersService;

        public ProductsController(
            IProductsService productsService,
            ICombosHelper combosHelper,
            INotyfService notyf,
            IUsersService usersService)
        {
            _productsService = productsService;
            _combosHelper = combosHelper;
            _notyf = notyf;
            _usersService = usersService;
        }

        [HttpGet]
        [CustomAuthorize(permission: "ShowProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<ProductDTO>> response = await _productsService.GetPaginationAsync(request);
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "CreateProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            ProductDTO dto = new ProductDTO
            {
                ProductCategories = await _combosHelper.GetComboProductCategories(),
            };
            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "CreateProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Create(ProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                return View(dto);
            }

            // Convertir el porcentaje a valor decimal
            if (dto.ProductTax.HasValue)
            {
                dto.ProductTax = dto.ProductTax.Value / 100;
            }

            Response<ProductDTO> response = await _productsService.CreateAsync(dto);
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            dto.ProductCategories = await _combosHelper.GetComboProductCategories();
            _notyf.Error(response.Message);
            return View(dto);
        }

        [HttpGet]
        [CustomAuthorize(permission: "UpdateProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                return RedirectToAction(nameof(Index));
            }

            Response<ProductDTO> response = await _productsService.GetOneAsync(id);

            if (!response.IsSuccess)
            {
                _notyf.Error(response.Message);
                return RedirectToAction(nameof(Index));
            }

            //Crear un nuevo objeto DTO para la vista
            ProductDTO dto = new ProductDTO
            {
                Id = response.Result.Id,
                ProductName = response.Result.ProductName,
                ProductDescription = response.Result.ProductDescription,
                ProductPrice = response.Result.ProductPrice,
                ProductBarCode = response.Result.ProductBarCode,
                ProductTax = response.Result.ProductTax * 100,
                IdProductCategory = response.Result.IdProductCategory,
                ProductCategories = await _combosHelper.GetComboProductCategories(response.Result.IdProductCategory),
            };

            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "UpdateProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Edit(ProductDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                dto.ProductCategories = await _combosHelper.GetComboProductCategories();
                return View(dto);
            }
            // Convertir el porcentaje a valor decimal
            if (dto.ProductTax.HasValue)
            {
                dto.ProductTax = dto.ProductTax.Value / 100;
            }

            Response<ProductDTO> response = await _productsService.EditAsync(dto);
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }

            dto.ProductCategories = await _combosHelper.GetComboProductCategories();
            _notyf.Error(response.Message);
            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "DeleteProduct", module: "Product")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<object> response = await _productsService.DeleteAsync(id);
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }
            else
            {
                _notyf.Error(response.Message);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers
{
    public class ProductCategoriesController : Controller
    {
        private readonly IProductCategoriesService _service;
        private readonly INotyfService _notyf;
        private readonly IUsersService _usersService;

        public ProductCategoriesController(
            IProductCategoriesService service,
            INotyfService notyf,
            IUsersService usersService)
        {
            _service = service;
            _notyf = notyf;
            _usersService = usersService;
        }
        [HttpGet]
        [CustomAuthorize(permission: "ShowProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Index([FromQuery] PaginationRequest request)
        {
            Response<PaginationResponse<ProductCategoryDTO>> response = await _service.GetPaginationAsync(request);
            return View(response.Result);
        }

        [HttpGet]
        [CustomAuthorize(permission: "CreateProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [CustomAuthorize(permission: "CreateProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Create(ProductCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                return View(dto);
            }
            Response<ProductCategoryDTO> response = await _service.CreateAsync(dto);

            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));

            }
            _notyf.Error(response.Message);
            return View(dto);
        }

        [HttpGet]
        [CustomAuthorize(permission: "UpdateProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                return RedirectToAction(nameof(Index));
            }
            Response<ProductCategoryDTO> response = await _service.GetOneAsync(id);
            if (response.IsSuccess)
            {
                return View(response.Result);
            }
            _notyf.Error(response.Message);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [CustomAuthorize(permission: "UpdateProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Edit(ProductCategoryDTO dto)
        {
            if (!ModelState.IsValid)
            {
                _notyf.Error("Debe ajustar los errores de validación");
                return View(dto);
            }
            Response<ProductCategoryDTO> response = await _service.EditAsync(dto);
            if (response.IsSuccess)
            {
                _notyf.Success(response.Message);
                return RedirectToAction(nameof(Index));
            }
            _notyf.Error(response.Message);
            return View(dto);
        }

        [HttpPost]
        [CustomAuthorize(permission: "DeleteProductCategory", module: "ProductCategory")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            Response<object> response = await _service.DeleteAsync(id);

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

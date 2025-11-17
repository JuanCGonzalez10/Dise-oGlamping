using Azure.Core;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductCategoriesController : ApiController
    {
        private readonly IProductCategoriesService _service;

        public ProductCategoriesController(IProductCategoriesService service)
        {
            _service = service;
        }

        [HttpGet]
        [ApiAuthorize(permission: "ShowProductCategory", module: "ProductCategory")]
        public async Task<IActionResult> GetProductsCategory([FromQuery] PaginationRequest request)
        {
            return ControllerBasicValidation(await _service.GetPaginationAsync(request));
        }
        [HttpGet("{id:int}")]
        [ApiAuthorize(permission: "ShowProductCategory", module: "ProductCategory")]
        public async Task<IActionResult> GetOneProductCategory([FromRoute] int id)
        {
            return ControllerBasicValidation(await _service.GetOneAsync(id));
        }

        [HttpPost]
        [ApiAuthorize(permission: "CreateProductCategory", module: "ProductCategory")]
        public async Task<IActionResult> CreateProductCategory([FromBody] ProductCategoryDTO dto)
        {
            return ControllerBasicValidation(await _service.CreateAsync(dto), ModelState);
        }
        [HttpPut]
        [ApiAuthorize(permission: "UpdateProductCategory", module: "ProductCategory")]
        public async Task<IActionResult> EditProductCategory([FromBody] ProductCategoryDTO dto)
        {
            return ControllerBasicValidation(await _service.EditAsync(dto), ModelState);
        }
        [HttpDelete("{id:int}")]
        [ApiAuthorize(permission: "DeleteProductCategory", module: "ProductCategory")]
        public async Task<IActionResult> DeleteProductCategory([FromRoute] int id)
        {
            return ControllerBasicValidation(await _service.DeleteAsync(id));
        }
    }
}

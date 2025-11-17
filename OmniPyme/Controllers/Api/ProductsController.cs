using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.Core.Attributes;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProductsController : ApiController
    {
        private readonly IProductsService _service;
        public ProductsController(IProductsService service)
        {
            _service = service;
        }
        [HttpGet]
        [ApiAuthorize(permission: "ShowProduct", module: "Product")]
        public async Task<IActionResult> GetProducts([FromQuery] PaginationRequest request)
        {
            return ControllerBasicValidation(await _service.GetPaginationAsync(request));
        }
        [HttpGet("{id:int}")]
        [ApiAuthorize(permission: "ShowProduct", module: "Product")]
        public async Task<IActionResult> GetOneProduct([FromRoute] int id)
        {
            return ControllerBasicValidation(await _service.GetOneAsync(id));
        }
        [HttpPost]
        [ApiAuthorize(permission: "CreateProduct", module: "Product")]
        public async Task<IActionResult> CreateProduct([FromBody] ProductDTO dto)
        {
            return ControllerBasicValidation(await _service.CreateAsync(dto), ModelState);
        }
        [HttpPut]
        [ApiAuthorize(permission: "UpdateProduct", module: "Product")]
        public async Task<IActionResult> EditProduct([FromBody] ProductDTO dto)
        {
            return ControllerBasicValidation(await _service.EditAsync(dto), ModelState);
        }
        [HttpDelete("{id:int}")]
        [ApiAuthorize(permission: "DeleteProduct", module: "Product")]
        public async Task<IActionResult> DeleteProduct([FromRoute] int id)
        {
            return ControllerBasicValidation(await _service.DeleteAsync(id));
        }
    }
}

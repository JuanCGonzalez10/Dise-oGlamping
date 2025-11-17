using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiAccountController : ApiController
    {
        private readonly IUsersService _usersService;

        public ApiAccountController(IUsersService usersService)
        {
            _usersService = usersService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            return ControllerBasicValidation(await _usersService.LoginApiAsync(dto));
        }
    }
}

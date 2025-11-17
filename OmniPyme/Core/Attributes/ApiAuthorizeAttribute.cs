using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Core.Attributes
{
    public class ApiAuthorizeAttribute : TypeFilterAttribute
    {
        public ApiAuthorizeAttribute(string permission, string module) : base(typeof(ApiAuthorizeFilter))
        {
            Arguments = [permission, module];
        }
    }

    public class ApiAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly string _module;
        private readonly IUsersService _usersService;

        public ApiAuthorizeFilter(string permission, string module, IUsersService usersService)
        {
            _permission = permission;
            _module = module;
            _usersService = usersService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            ClaimsPrincipal user = context.HttpContext.User;
            if (user?.Identity is null || !user.Identity.IsAuthenticated)
            {
                Response<object> response = ResponseHelper<object>.MakeResponseFail("No autenticado", ["El token no es válido o ha expirado."]);
                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status401Unauthorized
                };
                return;
            }

            bool isAuthorized = await _usersService.CurrentUserIsAuthorizedAsync(_permission, _module);

            if (!isAuthorized)
            {
                Response<object> response = ResponseHelper<object>.MakeResponseFail("Acceso denegado", ["No tienes permiso para acceder a este recurso."]);
                context.Result = new JsonResult(response)
                {
                    StatusCode = StatusCodes.Status403Forbidden
                };
            }
        }
    }
}

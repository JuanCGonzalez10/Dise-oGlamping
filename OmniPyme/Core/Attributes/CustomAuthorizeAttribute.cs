
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using OmniPyme.Web.Services;


namespace OmniPyme.Web.Core.Attributes
{
    public class CustomAuthorizeAttribute : TypeFilterAttribute
    {
        public CustomAuthorizeAttribute(string permission, string module) : base(typeof(CustomAuthorizeFilter))
        {
            Arguments = [permission, module];
        }
    }

    public class CustomAuthorizeFilter : IAsyncAuthorizationFilter
    {
        private readonly string _permission;
        private readonly string _module;
        private readonly IUsersService _usersService;

        public CustomAuthorizeFilter(string permission, string module, IUsersService usersService)
        {
            _permission = permission;
            _module = module;
            _usersService = usersService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            bool userIsAuthenticated = _usersService.CurrentUserIsAuthenticated();

            if (!userIsAuthenticated)
            {
                context.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "Account" },
                    { "Action", "Login" },
                    { "ReturnUrl", context.HttpContext.Request.Path }
                });

                return;
            }

            bool isAuthorized = await _usersService.CurrentUserIsAuthorizedAsync(_permission, _module);

            if (!isAuthorized)
            {
                context.Result = new ForbidResult();
            }
        }
    }
}

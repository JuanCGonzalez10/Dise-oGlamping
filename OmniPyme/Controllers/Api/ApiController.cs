using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OmniPyme.Web.Core;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        public static ObjectResult ControllerBasicValidation<T>(Response<T> response, ModelStateDictionary? modelState = null, int? statusCode = null)
        {
            if (modelState is not null && !modelState.IsValid)
            {
                List<string> errors = modelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return new ObjectResult(ResponseHelper<T>.MakeResponseFail("Debe ajustar los errores de validacion", errors))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
            }

            if (statusCode is not null)
            {
                return new ObjectResult(response)
                {
                    StatusCode = statusCode
                };
            }

            if (response.IsSuccess)
            {
                return new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status200OK
                };
            }

            return new ObjectResult(response)
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
    }
}

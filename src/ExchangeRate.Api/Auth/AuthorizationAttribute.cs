using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ExchangeRate.Abstraction.Data;
using ExchangeRate.Data.Data;

namespace ExchangeRate.Api.Auth
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // skip authorization if action is decorated with [AllowAnonymous] attribute
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
            if (allowAnonymous)
                return;

            StringValues headerValues;
            if (context.HttpContext.Request.Headers.TryGetValue("ApiKey", out headerValues))
            {
                var apiKey = headerValues.FirstOrDefault();

                if(String.IsNullOrEmpty(apiKey))
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }

                var unitOfWork = context.HttpContext.RequestServices.GetService<IUnitOfWork>();

                var repository = unitOfWork.GetRepository<CustomerApiKey>();

                var isExistingApiKey = repository.GetAsync(x => x.ApiKey == apiKey && x.IsActive).Result.Any();

                if (!isExistingApiKey)
                {
                    context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using ExchangeRate.Abstraction.Data;
using ExchangeRate.Data.Data;
using Microsoft.Extensions.Caching.Memory;

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
                var apiKey = headerValues.First();

                var memoryCache = context.HttpContext.RequestServices.GetService<IMemoryCache>();

                var apiKeyHasAccess = memoryCache.Get<bool?>(apiKey);

                if (apiKeyHasAccess == null)
                {
                    var unitOfWork = context.HttpContext.RequestServices.GetService<IUnitOfWork>();

                    var repository = unitOfWork.GetRepository<CustomerApiKey>();

                    var isExistingActiveApiKey = repository.GetAsync(x => x.ApiKey == apiKey && x.IsActive).Result.Any();

                    //Todo: can get from appsettings!
                    //warning. redis can be used also to break cache key in distributed system.
                    memoryCache.Set(apiKey, isExistingActiveApiKey, TimeSpan.FromMinutes(2));

                    apiKeyHasAccess = isExistingActiveApiKey;
                }

                if (!apiKeyHasAccess.Value)
                {
                    context.Result = new JsonResult(new { message = $"ApiKey: {apiKey} has no access!" }) { StatusCode = StatusCodes.Status401Unauthorized };
                    return;
                }
            }
            else
            {
                context.Result = new JsonResult(new { message = "No ApiKey Header supplied!" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}

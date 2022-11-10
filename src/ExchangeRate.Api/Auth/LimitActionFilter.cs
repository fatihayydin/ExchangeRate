using ExchangeRate.Abstraction.Data;
using ExchangeRate.Data.Data;
using ExchangeRate.Data.DataAccess;
using ExchangeRate.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace ExchangeRate.Api.Auth
{
    public class LimitActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var unitOfWork = context.HttpContext.RequestServices.GetService<IUnitOfWork>();

            var repository = unitOfWork.GetRepository<CustomerApiLog>();

            var apiKey = context.HttpContext.Request.Headers["ApiKey"].FirstOrDefault();

            var oldRecordsForApiKey = repository.GetAsync(x => x.ApiKey == apiKey && x.CreatedDate > DateTime.Now.AddHours(-1) && x.Direction == Direction.Outgoing && x.HttpStatusCode == (int)HttpStatusCode.OK);

            var count = oldRecordsForApiKey.Result.Count();

            if (count >= 10)
            {
                context.Result = new JsonResult(new { message = $"ApiKey: {apiKey} has requested too much!" }) { StatusCode = StatusCodes.Status429TooManyRequests };
                return;
            }
        }
    }
}

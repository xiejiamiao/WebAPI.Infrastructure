using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace WebAPI.Infrastructure.Gateway.Middelwares
{
    public class ExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandleMiddleware> _logger;
        private readonly bool _isOutputExceptionMessage;

        public ExceptionHandleMiddleware(RequestDelegate next,ILogger<ExceptionHandleMiddleware> logger,bool isOutputExceptionMessage)
        {
            _next = next;
            _logger = logger;
            _isOutputExceptionMessage = isOutputExceptionMessage;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await ExceptionHandler(httpContext, ex);
            }
        }

        private Task ExceptionHandler(HttpContext httpContext, Exception ex)
        {
            _logger.LogError(ex,ex.Message);
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            return httpContext.Response.WriteAsync(_isOutputExceptionMessage ? ex.Message : "Internal Server Error");
        }
    }
}
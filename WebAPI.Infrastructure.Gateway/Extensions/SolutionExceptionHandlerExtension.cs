using Microsoft.AspNetCore.Builder;
using WebAPI.Infrastructure.Gateway.Middelwares;

namespace WebAPI.Infrastructure.Gateway.Extensions
{
    public static class SolutionExceptionHandlerExtension
    {
        public static IApplicationBuilder UseSolutionExceptionHandler(this IApplicationBuilder builder,
            bool isOutputExceptionMessage)
        {
            builder.UseMiddleware<ExceptionHandleMiddleware>(isOutputExceptionMessage);
            return builder;
        }
    }
}
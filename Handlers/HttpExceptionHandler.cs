using Microsoft.AspNetCore.Diagnostics;

namespace DotnetDynamicExpressionsDemo.Handlers {
    public class HttpExceptionHandler : IExceptionHandler {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken) {
            if (exception is not BadHttpRequestException httpException) {
                return false;
            }

            httpContext.Response.StatusCode = httpException.StatusCode;

            await httpContext.Response.WriteAsJsonAsync(
                new {
                    error = httpException.Message
                },
                cancellationToken
            );

            return true;
        }
    }
}

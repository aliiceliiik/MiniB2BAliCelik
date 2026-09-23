using MiniB2B.Business.Common;

namespace MiniB2B.Web.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BusinessException exception)
        {
            _logger.LogWarning(exception, "İş kuralı hatası: {Path}", context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            if (context.Request.Path.StartsWithSegments("/api"))
                await context.Response.WriteAsJsonAsync(new { message = exception.Message });
            else
                context.Response.Redirect("/Home/Error");
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "İşlenmeyen hata: {Method} {Path}",
                context.Request.Method, context.Request.Path);

            if (context.Response.HasStarted)
                throw;

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            if (context.Request.Path.StartsWithSegments("/api"))
            {
                await context.Response.WriteAsJsonAsync(new
                {
                    message = "İşleminiz sırasında beklenmeyen bir hata oluştu. Lütfen tekrar deneyin."
                });
            }
            else
            {
                context.Response.Redirect("/Home/Error");
            }
        }
    }
}
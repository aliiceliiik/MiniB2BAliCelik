using Microsoft.AspNetCore.Authentication.Cookies;

namespace MiniB2B.Web.Auth;

public static class AuthServiceRegistration
{
    public static IServiceCollection AddCookieAuth(
        this IServiceCollection services, IWebHostEnvironment environment)
    {
        services
            .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(8);
                options.SlidingExpiration = true;

                options.Cookie.Name = "MiniB2B.Auth";
                options.Cookie.HttpOnly = true;
                options.Cookie.SameSite = SameSiteMode.Lax;
                options.Cookie.SecurePolicy = environment.IsDevelopment()
                    ? CookieSecurePolicy.SameAsRequest
                    : CookieSecurePolicy.Always;

                options.Events.OnRedirectToLogin = context =>
                    HandleRedirect(context.HttpContext, context.RedirectUri, StatusCodes.Status401Unauthorized);

                options.Events.OnRedirectToAccessDenied = context =>
                    HandleRedirect(context.HttpContext, context.RedirectUri, StatusCodes.Status403Forbidden);
            });

        return services;
    }

    private static Task HandleRedirect(HttpContext httpContext, string redirectUri, int apiStatusCode)
    {
        if (httpContext.Request.Path.StartsWithSegments("/api"))
            httpContext.Response.StatusCode = apiStatusCode;
        else
            httpContext.Response.Redirect(redirectUri);

        return Task.CompletedTask;
    }
}
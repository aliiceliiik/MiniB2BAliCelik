using Microsoft.Extensions.DependencyInjection;
using MiniB2B.Business.Services;
using MiniB2B.DataAccess;
using Microsoft.AspNetCore.Identity;
using MiniB2B.Entities.Models;

namespace MiniB2B.Business;

public static class BusinessServiceRegistration
{
    public static IServiceCollection AddBusiness(
        this IServiceCollection services, string connectionString)
    {
        services.AddDataAccess(connectionString);

        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IGridConfigService, GridConfigService>();
        services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IAuthService, AuthService>();

        return services;
    }
}
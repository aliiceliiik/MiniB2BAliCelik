using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MiniB2B.DataAccess.Context;
using MiniB2B.DataAccess.Repositories;


namespace MiniB2B.DataAccess;

public static class DataAccessServiceRegistration
{
    public static IServiceCollection AddDataAccess(
        this IServiceCollection services, string connectionString)
    {

     

        services.AddDbContext<MiniB2BDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IGridConfigRepository, GridConfigRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ICartRepository, CartRepository>();

        return services;
    }
}
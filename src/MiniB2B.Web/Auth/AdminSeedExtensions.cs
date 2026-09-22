using MiniB2B.Business.Seeding;

namespace MiniB2B.Web.Auth;

public static class AdminSeedExtensions
{
    public static async Task SeedAdminAsync(this WebApplication app)
    {
        var options = app.Configuration.GetSection("AdminSeed").Get<AdminSeedOptions>()
                      ?? new AdminSeedOptions();

        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IAdminSeeder>();
        await seeder.SeedAsync(options);
    }
}
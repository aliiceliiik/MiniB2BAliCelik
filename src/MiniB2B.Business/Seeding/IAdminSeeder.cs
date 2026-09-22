namespace MiniB2B.Business.Seeding;

public interface IAdminSeeder
{
    Task SeedAsync(AdminSeedOptions options);
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using MiniB2B.Business;
using MiniB2B.Web.Auth;
using MiniB2B.Web.Grid;
using MiniB2B.Web.Services;
using System.Text.Json.Serialization;
using MiniB2B.Web.Services;
using MiniB2B.Web.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("MiniB2B")
    ?? throw new InvalidOperationException("'MiniB2B' connection string bulunamadý.");

builder.Services.AddBusiness(connectionString);
builder.Services.AddGridRendering();
builder.Services.AddCookieAuth(builder.Environment);
builder.Services.AddSingleton<IImageStorage, LocalImageStorage>();

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter());
    options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());

    var messages = options.ModelBindingMessageProvider;
    messages.SetValueMustNotBeNullAccessor(_ => "Bu alan zorunludur.");
    messages.SetMissingBindRequiredValueAccessor(field => $"{field} alaný gönderilmedi.");
    messages.SetAttemptedValueIsInvalidAccessor((value, field) => $"'{value}' deðeri {field} alaný için geçerli deðil.");
    messages.SetValueIsInvalidAccessor(value => $"'{value}' deðeri geçerli deðil.");
    messages.SetNonPropertyAttemptedValueIsInvalidAccessor(value => $"'{value}' deðeri geçerli deðil.");
    messages.SetUnknownValueIsInvalidAccessor(field => $"{field} alanýna geçersiz bir deðer girildi.");
    messages.SetNonPropertyUnknownValueIsInvalidAccessor(() => "Geçersiz bir deðer girildi.");
    messages.SetMissingKeyOrValueAccessor(() => "Bu alan zorunludur.");
    messages.SetNonPropertyValueMustBeANumberAccessor(() => "Bu alana sayý girilmelidir.");
    messages.SetValueMustBeANumberAccessor(field => $"{field} alanýna sayý girilmelidir.");
})
    .AddJsonOptions(options =>
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddAntiforgery(options => options.HeaderName = "RequestVerificationToken");

var app = builder.Build();
await app.SeedAdminAsync();

// Configure the HTTP request pipeline.
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/Home/HttpError", "?code={0}");

app.UseHttpsRedirection();
app.UseRouting();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();

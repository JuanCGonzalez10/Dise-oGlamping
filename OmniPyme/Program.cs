using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.Data.Seeders;
using OmniPyme.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// 1. SERVICES
// --------------------------------------------------------

builder.Services.AddControllersWithViews();

// DB Context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));
});

// ⬇⬇⬇ ESTO FALTABA PARA QUE FUNCIONE EL SIDEBAR ⬇⬇⬇

// 👉 Necesario para acceder al usuario logueado
builder.Services.AddHttpContextAccessor();

// 👉 Registrar servicio de usuarios (NECESARIO para permisos)
builder.Services.AddScoped<IUsersService, UsersService>();

// 👉 Tus servicios personalizados
builder.Services.AddScoped<IReservationsService, ReservationsService>();

// Métodos personalizados
builder.AddCustomConfiguration();

// --------------------------------------------------------
// 2. BUILD APP
// --------------------------------------------------------

var app = builder.Build();

// --------------------------------------------------------
// 3. MIDDLEWARE
// --------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔐 Autenticación → SIEMPRE antes que Authorization
app.UseAuthentication();
app.UseAuthorization();

// Manejo de errores con páginas personalizadas
app.UseStatusCodePagesWithReExecute("/Errors/{0}");

// --------------------------------------------------------
// 4. ENDPOINTS
// --------------------------------------------------------

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    // Endpoint minimal extra
    endpoints.MapGet("/api/minimal", () =>
    {
        return "Hello from a minimal endpoint!";
    });
});

// Métodos personalizados del WebApplication
app.AddCustomWebApplicationConfiguration();

// --------------------------------------------------------
// 5. RUN
// --------------------------------------------------------

app.Run();

public partial class Program
{
    // ------------------------
    // MÉTODOS AUXILIARES
    // ------------------------

    async Task EnsureDatabaseMigrated(WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        await dbContext.Database.MigrateAsync();
    }

    async Task SeedData(WebApplication application)
    {
        using var scope = application.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<UserRolesSeeder>();
        await seeder.SeedAsync();
    }
}


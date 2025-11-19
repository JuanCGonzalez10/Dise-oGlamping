using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using AspNetCoreHero.ToastNotification.Notyf.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.Data.Seeders; // Necesario para la clase UserRolesSeeder
using OmniPyme.Web.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------------
// 1. SERVICES
// --------------------------------------------------------

builder.Services.AddControllersWithViews();

// DB Context
builder.Services.AddDbContext<DataContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"),
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure()); // Habilita reintentos
});

// 👉 Necesario para acceder al usuario logueado (para el IUsersService)
builder.Services.AddHttpContextAccessor();

// 👉 Registrar servicio de usuarios (NECESARIO para permisos)
builder.Services.AddScoped<IUsersService, UsersService>();

// 👉 Tus servicios personalizados (incluido el de Reservas)
builder.Services.AddScoped<IReservationsService, ReservationsService>();

// Inyección del Seeder (Debe ser Transient para ser inyectado y usado fuera del scope)
builder.Services.AddTransient<UserRolesSeeder>(); // Correcto

// Métodos personalizados
builder.AddCustomConfiguration();

builder.Services.AddNotyf(config =>
{
    config.DurationInSeconds = 10;
    config.IsDismissable = true;
    config.Position = NotyfPosition.TopRight;
    config.Position = NotyfPosition.TopRight;
});
builder.Services.AddScoped<IUsersService, UsersService>();

// --------------------------------------------------------
// 2. BUILD APP
// --------------------------------------------------------

WebApplication app = builder.Build();

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

// 🔐 Autenticación → SIEMPRE antes que Authorization
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.UseRouting();
app.UseAuthorization();

// 📢 Habilitar Middleware de Notyf
app.UseNotyf();

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

    endpoints.MapGet("/api/minimal", () =>
    {
        return "Hello from a minimal endpoint!";
    });
});

app.AddCustomWebApplicationConfiguration();

// --------------------------------------------------------
// 5. MIGRACIÓN Y SEEDING (¡CRUCIAL!)
// --------------------------------------------------------

// 1. FORZAR MIGRACIONES ANTES DEL SEEDER
await EnsureDatabaseIsMigrated(app);

// 2. LLAMADA AL SEEDER
await SeedData(app);

// --------------------------------------------------------
// 6. RUN
// --------------------------------------------------------

app.Run();

// FUNCIÓN PARA APLICAR MIGRACIONES
async Task EnsureDatabaseIsMigrated(WebApplication application)
{
    // Usa un scope para acceder a servicios
    using (var scope = application.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
        // Esto crea la DB si no existe y aplica todas las migraciones pendientes
        await dbContext.Database.MigrateAsync();
    }
}

// FUNCIÓN DEL SEEDER
async Task SeedData(WebApplication application)
{
    // Usa un scope para acceder a servicios
    using (IServiceScope scope = application.Services.CreateScope())
    {
        UserRolesSeeder seeder = scope.ServiceProvider.GetRequiredService<UserRolesSeeder>();
        await seeder.SeedAsync();
    }
}

public partial class Program { }


using System.Text;
using AspNetCoreHero.ToastNotification;
using AspNetCoreHero.ToastNotification.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.Data.Seeders;
using OmniPyme.Web.Helpers;
using OmniPyme.Web.Services;
using Serilog;

namespace OmniPyme.Web
{
    public static class CustomConfiguration
    {
        public static WebApplicationBuilder AddCustomConfiguration(this WebApplicationBuilder builder)
        {
            // Data Context
            builder.Services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection"));
            });

            builder.Services.AddHttpContextAccessor();

            //AutoMapper
            builder.Services.AddAutoMapper(typeof(Program));

            //Services
            AddServices(builder);

            //Identoty an Acces Managment
            AddIAM(builder);

            // Toast Notification SetUp
            builder.Services.AddNotyf(config =>
            {
                config.DurationInSeconds = 10;
                config.IsDismissable = true;
                config.Position = NotyfPosition.BottomRight;
            });

            //Log setup
            AddLogConfiguration(builder);

            //Desactivar autovalidacion del ModelState en API
            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
            });

            return builder;
        }

        private static void AddLogConfiguration(WebApplicationBuilder builder)
        {
            Log.Logger = new LoggerConfiguration().WriteTo.File("logs/log.log",
                                                                rollingInterval: RollingInterval.Day,
                                                                restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Warning)
                                                  .WriteTo.Console(restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Information)
                                                  .CreateLogger();
            builder.Logging.ClearProviders();
            builder.Logging.AddSerilog();
        }

        private static void AddServices(WebApplicationBuilder builder)
        {
            //Services
            builder.Services.AddScoped<IApiService, ApiService>();
            builder.Services.AddScoped<IClientsService, ClientsService>();
            builder.Services.AddScoped<IEmailService, MailtrapService>();
            builder.Services.AddScoped<IHomeService, HomeService>();
            builder.Services.AddScoped<IInvoicesService, InvoicesService>();
            builder.Services.AddScoped<IProductCategoriesService, ProductCategoriesService>();
            builder.Services.AddScoped<IProductsService, ProductsService>();
            builder.Services.AddTransient<IReadLogsService, ReadPlainTextLogsService>();
            builder.Services.AddScoped<IRolesService, RolesService>();
            builder.Services.AddScoped<ISalesService, SalesService>();
            builder.Services.AddScoped<ISaleDetailService, SaleDetailService>();
            builder.Services.AddTransient<SeedDb>();
            builder.Services.AddScoped<IUsersService, UsersService>();

            //Storage
            builder.Services.AddKeyedScoped<IStorageService, LocalStorageService>("local");
            builder.Services.AddKeyedScoped<IStorageService, AzureInvoiceStorageService>("azure");

            //Helpers
            builder.Services.AddScoped<ICombosHelper, CombosHelper>();
        }

        public static WebApplication AddCustomWebApplicationConfiguration(this WebApplication app)
        {
            app.UseNotyf();

            SeedData(app);

            return app;
        }

        private static void SeedData(WebApplication app)
        {
            IServiceScopeFactory scopeFactory = app.Services.GetService<IServiceScopeFactory>();

            using (IServiceScope scope = scopeFactory.CreateScope())
            {
                SeedDb service = scope.ServiceProvider.GetService<SeedDb>();
                //service.SeedAsync().Wait();
            }
        }

        private static void AddIAM(WebApplicationBuilder builder)
        {
            builder.Services.AddIdentity<Users, IdentityRole>(conf =>
            {
                conf.User.RequireUniqueEmail = true;
                conf.Password.RequireDigit = false;
                conf.Password.RequiredUniqueChars = 0;
                conf.Password.RequireLowercase = false;
                conf.Password.RequireUppercase = false;
                conf.Password.RequireNonAlphanumeric = false;
                conf.Password.RequiredLength = 4;
            }).AddEntityFrameworkStores<DataContext>()
              .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(conf =>
            {
                conf.Cookie.Name = "Auth";
                conf.ExpireTimeSpan = TimeSpan.FromDays(100);
                conf.LoginPath = "/Account/Login";
                conf.AccessDeniedPath = "/Errors/403";
            });

            builder.Services.AddAuthentication()
                            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                            {
                                options.TokenValidationParameters = new TokenValidationParameters
                                {
                                    ValidateIssuer = true,
                                    ValidateAudience = true,
                                    ValidateLifetime = true,
                                    ValidateIssuerSigningKey = true,
                                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                                    ValidAudience = builder.Configuration["Jwt:Audience"],
                                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                                };
                            });
        }
    }
}

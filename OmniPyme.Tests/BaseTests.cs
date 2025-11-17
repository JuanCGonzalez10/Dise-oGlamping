using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using OmniPyme.Data;
using OmniPyme.Web.Core;

namespace OmniPyme.Tests
{
    public class BaseTests
    {
        protected DataContext BuildContext(string dbName)
        {
            DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>().UseInMemoryDatabase(dbName).Options;
            DataContext dataContext = new DataContext(options);
            return dataContext;
        }

        protected IMapper ConfigureAutoMapper()
        {
            MapperConfiguration config = new MapperConfiguration(options =>
            {
                options.AddProfile(new AutoMapperProfiles());
            });

            return config.CreateMapper();
        }

        protected WebApplicationFactory<Program> BuildWebApplicationFactory(string dbName)
        {
            WebApplicationFactory<Program> factory = new WebApplicationFactory<Program>();

            factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    ServiceDescriptor? descriptorDbContext = services.SingleOrDefault(d =>
                        d.ServiceType == typeof(DbContextOptions<DataContext>));

                    if (descriptorDbContext is not null)
                    {
                        services.Remove(descriptorDbContext);
                    }

                    services.AddDbContext<DataContext>(options => options.UseInMemoryDatabase(dbName));

                });
            });

            return factory;
        }
    }
}

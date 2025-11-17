using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.Data.Seeders
{

    public class PermissionSeeder
    {
        private readonly DataContext _context;

        public PermissionSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {

            List<Permission> permissions = [.. Client(), .. ProductCategories(), .. Products(), .. Sales(), .. Roles(), .. Users(), .. Logs()];

            foreach (Permission permission in permissions)
            {
                bool exists = await _context.Permissions.AnyAsync(p => p.Name == permission.Name && p.Module == permission.Module);

                if (!exists)
                {

                    await _context.Permissions.AddAsync(permission);
                }

            }

            await _context.SaveChangesAsync();
        }

        private List<Permission> Client()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowClient", Description = "Ver clientes", Module = "Client" },
                    new Permission { Name = "CreateClient", Description = "Crear clientes", Module = "Client" },
                    new Permission { Name = "UpdateClient", Description = "Editar clientes", Module = "Client" },
                    new Permission { Name = "DeleteClient", Description = "Eliminar clientes", Module = "Client" },


                };
        }

        private List<Permission> ProductCategories()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowProductCategory", Description = "Ver categorías de productos", Module = "ProductCategory" },
                    new Permission { Name = "CreateProductCategory", Description = "Crear categorías de productos", Module = "ProductCategory" },
                    new Permission { Name = "UpdateProductCategory", Description = "Editar categorías de productos", Module = "ProductCategory" },
                    new Permission { Name = "DeleteProductCategory", Description = "Eliminar categorías de productos", Module = "ProductCategory" },


                };
        }


        private List<Permission> Products()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowProduct", Description = "Ver productos", Module = "Product" },
                    new Permission { Name = "CreateProduct", Description = "Crear productos", Module = "Product" },
                    new Permission { Name = "UpdateProduct", Description = "Editar productos", Module = "Product" },
                    new Permission { Name = "DeleteProduct", Description = "Eliminar productos", Module = "Product" },


                };
        }


        private List<Permission> Sales()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowSale", Description = "Ver ventas", Module = "Sale" },
                    new Permission { Name = "CreateSale", Description = "Crear ventas", Module = "Sale" },
                    new Permission { Name = "ViewSale", Description = "Ver ventas", Module = "Sale" },
                    new Permission { Name = "DeleteSale", Description = "Eliminar ventas", Module = "Sale" },


                };
        }

        private List<Permission> Roles()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowRoles", Description = "Ver roles", Module = "Roles" },
                    new Permission { Name = "CreateRoles", Description = "Crear roles", Module = "Roles" },
                    new Permission { Name = "UpdateRoles", Description = "Editar roles", Module = "Roles" },
                    new Permission { Name = "DeleteRoles", Description = "Eliminar roles", Module = "Roles" },


                };
        }

        private List<Permission> Users()
        {

            return new List<Permission>
                {
                    new Permission { Name = "ShowUsers", Description = "Ver Users", Module = "Users" },
                    new Permission { Name = "CreateUsers", Description = "Crear Users", Module = "Users" },
                    new Permission { Name = "UpdateUsers", Description = "Editar Users", Module = "Users" },
                    new Permission { Name = "DeleteUsers", Description = "Eliminar Users", Module = "Users" }
                };
        }

        private List<Permission> Logs()
        {
            return new List<Permission>
                {
                    new Permission { Name = "ShowLogs", Description = "Ver Logs", Module = "Logs" },
                };
        }
    }
}

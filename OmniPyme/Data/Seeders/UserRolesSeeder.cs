using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.Services;
using System.Drawing.Text;
using System.Numerics;

namespace OmniPyme.Web.Data.Seeders
{
    public class UserRolesSeeder
    {
        private readonly DataContext _context;
        private readonly IUsersService _usersService;

        public UserRolesSeeder(DataContext context, IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        public async Task SeedAsync()
        {
            await CheckRoles();
            //await BasicRoleAsync();
            await CheckUsers();

        }

        //private async Task BasicRoleAsync()
        //{
        //    bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == "Basic");

        //    if (!exists)
        //    {
        //        PrivateURole role = new PrivateURole { Name = "Basic" };
        //        await _context.PrivateURoles.AddAsync(role);
        //        await _context.SaveChangesAsync();
        //    }
        //}

        private async Task CheckUsers()
        {
            Users? users = await _usersService.GetUserAsync("Admin@gmail.com");

            if (users is null)
            {
                PrivateURole adminRole = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Name == Env.SUPER_ADMIN_ROLE_NAME);

                users = new Users
                {
                    Email = "AdminJuan@gmail.com",
                    FirstName = "Super",
                    LastName = "Admin",
                    PhoneNumber = "400000000",
                    UserName = "AdminJuan@gmail.com",
                    Document = "11111122",
                    Photo = "https://localhost:7045/users/0fb1b2a9-992a-4992-b70e-ee409baf034a.jpg",
                    PrivateURole = adminRole
                };

                await _usersService.AddUserAsync(users, "12345");

                string token = await _usersService.GenerateEmailConfirmationTokenAsync(users);
                await _usersService.ConfirmEmailAsync(users, token);

            }
            // Gerente
            users = await _usersService.GetUserAsync("gerente@gmail.com");

            if (users is null)
            {
                PrivateURole gerenteRole = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Name == "Gerente");

                users = new Users
                {
                    Email = "gerenteAleja@gmail.com",
                    FirstName = "Gerente",
                    LastName = "Gerente",
                    PhoneNumber = "9876",
                    UserName = "gerente@gmail.com",
                    Document = "56789",
                    Photo = "https://localhost:7045/users/0fb1b2a9-992a-4992-b70e-ee409baf034a.jpg",
                    PrivateURole = gerenteRole
                };
                await _usersService.AddUserAsync(users, "1234");
                string token = await _usersService.GenerateEmailConfirmationTokenAsync(users);
                await _usersService.ConfirmEmailAsync(users, token);
            }
            // Vendedor
            users = await _usersService.GetUserAsync("vendedor@yopmail.com");

            if (users is null)
            {
                PrivateURole vendedorRole = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Name == "Vendedor");

                users = new Users
                {
                    Email = "vendedor@gmail.com",
                    FirstName = "Vendedor",
                    LastName = "Vendedor",
                    PhoneNumber = "345465347",
                    UserName = "vendedor@gmail.com",
                    Document = "1767234",
                    Photo = "https://localhost:7045/users/0fb1b2a9-992a-4992-b70e-ee409baf034a.jpg",
                    PrivateURole = vendedorRole
                };
                await _usersService.AddUserAsync(users, "1234");
                string token = await _usersService.GenerateEmailConfirmationTokenAsync(users);
                await _usersService.ConfirmEmailAsync(users, token);
            }
            // Inventario
            users = await _usersService.GetUserAsync("inventario@yopmail.com");

            if (users is null)
            {
                PrivateURole inventarioRole = await _context.PrivateURoles.FirstOrDefaultAsync(r => r.Name == "Gestor de Inventario");

                users = new Users
                {
                    Email = "inventario@gmail.com",
                    FirstName = "Gestor",
                    LastName = "Inventario",
                    PhoneNumber = "345462345347",
                    UserName = "inventario@gmail.com",
                    Document = "17672",
                    Photo = "https://localhost:7045/users/0fb1b2a9-992a-4992-b70e-ee409baf034a.jpg",
                    PrivateURole = inventarioRole
                };
                await _usersService.AddUserAsync(users, "1234");
                string token = await _usersService.GenerateEmailConfirmationTokenAsync(users);
                await _usersService.ConfirmEmailAsync(users, token);
            }
            // CLIENTE (Turista)
            users = await _usersService.GetUserAsync("cliente@gmail.com");

            if (users is null)
            {
                PrivateURole turistaRole = await _context.PrivateURoles
                    .FirstOrDefaultAsync(r => r.Name == "Turista"); // Usar "Turista"

                users = new Users
                {
                    Email = "Turista@gmail.com",
                    FirstName = "Turista",
                    LastName = "Turista",
                    UserName = "Turista@gmail.com",
                    PhoneNumber = "000111222",
                    Document = "101010110",
                    PrivateURole = turistaRole,
                    Photo = "https://localhost:7045/users/10771687.png"
                };

                await _usersService.AddUserAsync(users, "1234"); // Contraseña segura
                string token = await _usersService.GenerateEmailConfirmationTokenAsync(users);
                await _usersService.ConfirmEmailAsync(users, token);
            }

        }
        private async Task CheckRoles()
        {
            await AdminRolesAsync();
            await ManagerRoleAsync();
            await VendorRoleAsync();
            await InventoryManagerRoleAsync();
            await TuristaRoleAsync();
        }

        private async Task ManagerRoleAsync()
        {
            bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == "Gerente");

            if (!exists)
            {
                PrivateURole role = new PrivateURole { Name = "Gerente" };
                await _context.PrivateURoles.AddAsync(role);

                List<Permission> permissions = await _context.Permissions.Where(p => p.Module == "Client" || p.Module == "Product" || p.Module == "ProductCategory" || p.Module == "Sale" || p.Module == "Users")
                                                                         .ToListAsync();


                foreach (Permission permission in permissions)
                {

                    await _context.RolePermissions.AddAsync(new RolePermission { Permission = permission, Role = role });

                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task VendorRoleAsync()
        {
            bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == "Vendedor");

            if (!exists)
            {
                PrivateURole role = new PrivateURole { Name = "Vendedor" };
                await _context.PrivateURoles.AddAsync(role);

                List<Permission> permissions = await _context.Permissions.Where(p => (p.Module == "Client" || p.Module == "Sale") && !p.Name.StartsWith("Delete"))
                                                                         .ToListAsync();


                foreach (Permission permission in permissions)
                {

                    await _context.RolePermissions.AddAsync(new RolePermission { Permission = permission, Role = role });

                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task InventoryManagerRoleAsync()
        {
            bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == "Gestor de Inventario");

            if (!exists)
            {
                PrivateURole role = new PrivateURole { Name = "Gestor de Inventario" };
                await _context.PrivateURoles.AddAsync(role);

                List<Permission> permissions = await _context.Permissions.Where(p => (p.Module == "Product" || p.Module == "ProductCategory") && !p.Name.StartsWith("Delete"))
                                                                         .ToListAsync();


                foreach (Permission permission in permissions)
                {

                    await _context.RolePermissions.AddAsync(new RolePermission { Permission = permission, Role = role });

                }

                await _context.SaveChangesAsync();
            }
        }

        private async Task AdminRolesAsync()
        {
            bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == Env.SUPER_ADMIN_ROLE_NAME);

            if (!exists)
            {
                PrivateURole role = new PrivateURole { Name = Env.SUPER_ADMIN_ROLE_NAME };
                await _context.PrivateURoles.AddAsync(role);
                await _context.SaveChangesAsync();
            }
        }

        // --- MÉTODO REFACTORIZADO Y CORREGIDO PARA TURISTA ---
        private async Task TuristaRoleAsync()
        {
            const string roleName = "Turista";

            bool exists = await _context.PrivateURoles.AnyAsync(r => r.Name == roleName);

            if (!exists)
            {
                PrivateURole role = new PrivateURole { Name = roleName };
                await _context.PrivateURoles.AddAsync(role);

                
                // - Ver Glampings ("ShowProduct")
                // - Crear Reserva desde Producto ("CreateReservationFromProduct")
                // - Ver Mis Reservas ("ShowMyReservations")
                // - Eliminar Reserva ("DeleteReservation")

                List<Permission> permissions = await _context.Permissions
                    .Where(p =>
                        (p.Module == "Product" && p.Name == "ShowProduct") ||
                        (p.Module == "Reservation" &&
                         (p.Name == "CreateReservationFromProduct" ||
                          p.Name == "ShowReservation" ||
                          p.Name == "DeleteReservation") // Incluido el permiso de eliminar
                        )
                    )
                    .ToListAsync();

                // 2. Asignar los permisos al rol
                foreach (Permission permission in permissions)
                {
                    await _context.RolePermissions.AddAsync(new RolePermission { Permission = permission, Role = role });
                }

                await _context.SaveChangesAsync();
            }
        }


    }

}


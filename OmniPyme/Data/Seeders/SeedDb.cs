using OmniPyme.Data;
using OmniPyme.Web.Services;

namespace OmniPyme.Web.Data.Seeders
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUsersService _usersService;

        public SeedDb(DataContext context, IUsersService usersService)
        {
            _context = context;
            _usersService = usersService;
        }

        public async Task SeedAsync()
        {
            await new ClientSeeder(_context).SeedAsync();
            await new SaleSeeder(_context).SeedAsync();
            await new InvoiceSeeder(_context).SeedAsync();
            await new ProductCategorySeeder(_context).SeedAsync();
            await new ProductSeeder(_context).SeedAsync();
            await new SaleDetailSeeder(_context).SeedAsync();
            await new PermissionSeeder(_context).SeedAsync();
            await new UserRolesSeeder(_context, _usersService).SeedAsync();



        }
    }
}

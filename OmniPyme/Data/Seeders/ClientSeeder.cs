using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.Data.Seeders
{
    public class ClientSeeder
    {
        private readonly DataContext _context;

        public ClientSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Client> clients = new List<Client>
            {
                new Client
                {
                    DNI = "1234567890",
                    FirstName = "Juan",
                    LastName = "Perez",
                    Email = "juanperez@correo.com",
                    Phone = "3214569874",
                    RegisterDate = DateTime.Parse("2025-01-01 00:00:00"),
                    LastPurchaseDate = DateTime.Parse("2025-03-23 00:00:00")
                },
                new Client
                {
                    DNI = "1234567891",
                    FirstName = "Carlos",
                    LastName = "Martinez",
                    Email = "carlosmartinez@correo.com",
                    Phone = "3124665724",
                    RegisterDate = DateTime.Parse("2025-03-24 15:07:56.678"),
                    LastPurchaseDate = DateTime.Parse("2025-03-24 15:07:56.678")
                },
                new Client
                {
                    DNI = "1234567892",
                    FirstName = "Manuel",
                    LastName = "Garcia",
                    Email = "manuelgarcia@correo.com.co",
                    Phone = "3114567513",
                    RegisterDate = DateTime.Parse("2025-03-24 15:37:46.243"),
                    LastPurchaseDate = DateTime.Parse("2025-03-24 15:37:46.243")
                },
                new Client
                {
                    DNI = "1234567893",
                    FirstName = "Angel",
                    LastName = "Bermudez",
                    Email = "angelbermudez@correo.com",
                    Phone = "1321456785",
                    RegisterDate = DateTime.Parse("2025-03-29 19:35:42.375"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:35:42.375")
                },
                new Client
                {
                    DNI = "1234567894",
                    FirstName = "Maximiliano",
                    LastName = "de la Rosa",
                    Email = "maximiliano@correo.com",
                    Phone = "4567821564",
                    RegisterDate = DateTime.Parse("2025-03-29 19:36:58.827"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:36:58.827")
                },
                new Client
                {
                    DNI = "1234567895",
                    FirstName = "Cristina",
                    LastName = "Moncada",
                    Email = "cristinamoncada@correo.com",
                    Phone = "4521031497",
                    RegisterDate = DateTime.Parse("2025-03-29 19:37:33.999"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:37:33.999")
                },
                new Client
                {
                    DNI = "1234567896",
                    FirstName = "Monica",
                    LastName = "Gutierrez",
                    Email = "monicagutierrez@correo.com",
                    Phone = "6452103145",
                    RegisterDate = DateTime.Parse("2025-03-29 19:38:01.273"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:38:01.273")
                },
                new Client
                {
                    DNI = "1234567897",
                    FirstName = "Sandra",
                    LastName = "Toro",
                    Email = "sandratoro@correo.com",
                    Phone = "6987541230",
                    RegisterDate = DateTime.Parse("2025-03-29 19:38:31.572"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:38:31.572")
                },
                new Client
                {
                    DNI = "1234567898",
                    FirstName = "Alina",
                    LastName = "Moncada",
                    Email = "alinamoncada@correo.com",
                    Phone = "9875632145",
                    RegisterDate = DateTime.Parse("2025-03-29 19:38:59.538"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:38:59.538")
                },
                new Client
                {
                    DNI = "1234567899",
                    FirstName = "Gloria",
                    LastName = "Arboleda",
                    Email = "gloriaarboleda@correo.com",
                    Phone = "4658951234",
                    RegisterDate = DateTime.Parse("2025-03-29 19:42:02.588"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 19:42:02.588")
                },
                new Client
                {
                    DNI = "1234567900",
                    FirstName = "John",
                    LastName = "Doe",
                    Email = "johndoe@correo.com",
                    Phone = "1234567890",
                    RegisterDate = DateTime.Parse("2025-03-29 22:00:43.593"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 22:00:43.593")
                },
                new Client
                {
                    DNI = "1234567901",
                    FirstName = "Jane",
                    LastName = "Doe",
                    Email = "janeDoe@correo.com",
                    Phone = "1234567890",
                    RegisterDate = DateTime.Parse("2025-03-29 22:00:43.593"),
                    LastPurchaseDate = DateTime.Parse("2025-03-29 22:00:43.593")
                }
            };

            foreach (Client client in clients)
            {
                bool exists = await _context.Clients.AnyAsync(c => c.Email == client.Email);

                if (!exists)
                {
                    await _context.Clients.AddAsync(client);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

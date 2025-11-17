using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.Data.Seeders
{
    public class ProductSeeder
    {
        private readonly DataContext _context;

        public ProductSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Product> products = new List<Product>
            {
                new Product
                {
                    ProductName = "Laptop",
                    ProductDescription = "Laptop de alta gama",
                    ProductPrice = 1500.00m,
                    ProductBarCode = "1234567890123",
                    ProductTax = 0.19,
                    IdProductCategory = 1,
                },
                new Product
                {
                    ProductName = "Camisa",
                    ProductDescription = "Camisa de algodón",
                    ProductPrice = 25.00m,
                    ProductBarCode = "1234567890124",
                    ProductTax = 0.19,
                    IdProductCategory = 2,
                },
                new Product
                {
                    ProductName = "Sofá",
                    ProductDescription = "Sofá de cuero",
                    ProductPrice = 800.00m,
                    ProductBarCode = "1234567890125",
                    ProductTax = 0.19,
                    IdProductCategory = 3,
                }
            };
            foreach (Product product in products)
            {
                bool exists = await _context.Products.AnyAsync(x => x.ProductBarCode == product.ProductBarCode);
                if (!exists)
                {
                    await _context.Products.AddAsync(product);
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}

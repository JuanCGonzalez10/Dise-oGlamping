using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.Data.Seeders
{
    public class InvoiceSeeder
    {

        private readonly DataContext _context;

        public InvoiceSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<Invoice> invoices = new List<Invoice>
            {
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-01-15 10:30:00"),
                    InvoiceNumber = "FAC-2025-0001",
                    IdSale = 1
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-01-28 14:45:00"),
                    InvoiceNumber = "FAC-2025-0002",
                    IdSale = 2
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-02-05 09:15:00"),
                    InvoiceNumber = "FAC-2025-0003",
                    IdSale = 3
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-02-12 16:20:00"),
                    InvoiceNumber = "FAC-2025-0004",
                    IdSale = 4
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-02-27 11:40:00"),
                    InvoiceNumber = "FAC-2025-0005",
                    IdSale = 5
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-03-08 13:05:00"),
                    InvoiceNumber = "FAC-2025-0006",
                    IdSale = 6
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-03-14 15:30:00"),
                    InvoiceNumber = "FAC-2025-0007",
                    IdSale = 7
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-03-22 10:00:00"),
                    InvoiceNumber = "FAC-2025-0008",
                    IdSale = 8
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-03-29 17:15:00"),
                    InvoiceNumber = "FAC-2025-0009",
                    IdSale = 9
                },
                new Invoice
                {
                    InvoiceDate = DateTime.Parse("2025-04-01 12:25:00"),
                    InvoiceNumber = "FAC-2025-0010",
                    IdSale = 10
                }
            };

            foreach (Invoice invoice in invoices)
            {
                bool exists = await _context.Invoices.AnyAsync(i => i.InvoiceNumber == invoice.InvoiceNumber);

                if (!exists)
                {
                    await _context.Invoices.AddAsync(invoice);
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}

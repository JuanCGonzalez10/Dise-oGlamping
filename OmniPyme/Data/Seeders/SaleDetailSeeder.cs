using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Data.Entities;

namespace OmniPyme.Web.Data.Seeders
{
    public class SaleDetailSeeder
    {
        private readonly DataContext _context;

        public SaleDetailSeeder(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            List<SaleDetail> saleDetails = new List<SaleDetail>
            {
                new SaleDetail
                {
                    SaleDetailCode = "V001-1",
                    SaleDetailProductQuantity = 2,
                    SaleDetailProductPrice = 1500.00m,
                    SaleDetailSubtotal = 3570.00m,
                    SaleDetailProductCode = 1,
                    SaleDetailProductTax = 570.00m,
                    IdSale = 1
                },
                new SaleDetail
                {
                    SaleDetailCode = "V001-2",
                    SaleDetailProductQuantity = 1,
                    SaleDetailProductPrice = 25.00m,
                    SaleDetailSubtotal = 29.75m,
                    SaleDetailProductCode = 2,
                    SaleDetailProductTax = 4.75m,
                    IdSale = 1
                },
                new SaleDetail
                {
                    SaleDetailCode = "V002-1",
                    SaleDetailProductQuantity = 3,
                    SaleDetailProductPrice = 800.00m,
                    SaleDetailSubtotal = 2856.00m,
                    SaleDetailProductCode = 3,
                    SaleDetailProductTax = 456.00m,
                    IdSale = 2
                },
                new SaleDetail
                {
                    SaleDetailCode = "V003-1",
                    SaleDetailProductQuantity = 2,
                    SaleDetailProductPrice = 1500.00m,
                    SaleDetailSubtotal = 3570.00m,
                    SaleDetailProductCode = 1,
                    SaleDetailProductTax = 570.00m,
                    IdSale = 3
                },
                new SaleDetail
                {
                    SaleDetailCode = "V004-1",
                    SaleDetailProductQuantity = 1,
                    SaleDetailProductPrice = 25.00m,
                    SaleDetailSubtotal = 29.75m,
                    SaleDetailProductCode = 2,
                    SaleDetailProductTax = 4.75m,
                    IdSale = 4
                },
                new SaleDetail
                {
                    SaleDetailCode = "V005-1",
                    SaleDetailProductQuantity = 4,
                    SaleDetailProductPrice = 800.00m,
                    SaleDetailSubtotal = 3808.00m,
                    SaleDetailProductCode = 3,
                    SaleDetailProductTax = 608.00m,
                    IdSale = 5
                },
                new SaleDetail
                {
                    SaleDetailCode = "V006-1",
                    SaleDetailProductQuantity = 1,
                    SaleDetailProductPrice = 1500.00m,
                    SaleDetailSubtotal = 1785.00m,
                    SaleDetailProductCode = 1,
                    SaleDetailProductTax = 285.00m,
                    IdSale = 6
                },
                new SaleDetail
                {
                    SaleDetailCode = "V007-1",
                    SaleDetailProductQuantity = 2,
                    SaleDetailProductPrice = 25.00m,
                    SaleDetailSubtotal = 34.50m,
                    SaleDetailProductCode = 2,
                    SaleDetailProductTax = 9.50m,
                    IdSale = 7
                },
                new SaleDetail
                {
                    SaleDetailCode = "V008-1",
                    SaleDetailProductQuantity = 3,
                    SaleDetailProductPrice = 800.00m,
                    SaleDetailSubtotal = 2856.00m,
                    SaleDetailProductCode = 3,
                    SaleDetailProductTax = 456.00m,
                    IdSale = 8
                },
                new SaleDetail
                {
                    SaleDetailCode = "V009-1",
                    SaleDetailProductQuantity = 2,
                    SaleDetailProductPrice = 1500.00m,
                    SaleDetailSubtotal = 3570.00m,
                    SaleDetailProductCode = 1,
                    SaleDetailProductTax = 570.00m,
                    IdSale = 9
                },
                new SaleDetail
                {
                    SaleDetailCode = "V010-1",
                    SaleDetailProductQuantity = 1,
                    SaleDetailProductPrice = 25.00m,
                    SaleDetailSubtotal = 29.75m,
                    SaleDetailProductCode = 2,
                    SaleDetailProductTax = 4.75m,
                    IdSale = 10
                },
                new SaleDetail
                {
                    SaleDetailCode = "V010-2",
                    SaleDetailProductQuantity = 4,
                    SaleDetailProductPrice = 800.00m,
                    SaleDetailSubtotal = 3808.00m,
                    SaleDetailProductCode = 3,
                    SaleDetailProductTax = 608.00m,
                    IdSale = 10
                },
                new SaleDetail
                {
                    SaleDetailCode = "V010-3",
                    SaleDetailProductQuantity = 2,
                    SaleDetailProductPrice = 1500.00m,
                    SaleDetailSubtotal = 3570.00m,
                    SaleDetailProductCode = 1,
                    SaleDetailProductTax = 570.00m,
                    IdSale = 10
                },
            };

            foreach (SaleDetail detail in saleDetails)
            {
                bool exists = await _context.SaleDetails.AnyAsync(sd =>
                    sd.SaleDetailCode == detail.SaleDetailCode);

                if (!exists)
                {
                    await _context.SaleDetails.AddAsync(detail);
                }

                await _context.SaveChangesAsync();

            }
        }
    }
}

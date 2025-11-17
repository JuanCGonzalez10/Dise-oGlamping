using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using OmniPyme.Data;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.Data.Entities;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Tests.UnitTests.Services
{
    [TestClass]
    public class ProductServiceTests : BaseTests
    {
        [TestMethod]
        public async Task GetPagination_ReturnsAllProducts()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();

            context.AddRange(new List<Product>
            {
                new Product { ProductName = "Product 1", ProductDescription = "Description 1", ProductPrice = 10.0m, ProductBarCode = "1234567890123", ProductTax = 5.0, IdProductCategory = 1 },
                new Product { ProductName = "Product 2", ProductDescription = "Description 2", ProductPrice = 20.0m, ProductBarCode = "1234567890124", ProductTax = 10.0, IdProductCategory = 1 },
                new Product { ProductName = "Product 3", ProductDescription = "Description 3", ProductPrice = 30.0m, ProductBarCode = "1234567890125", ProductTax = 15.0, IdProductCategory = 1 }
            });

            await context.SaveChangesAsync();

            // Act
            DataContext context2 = BuildContext(dbName);
            IProductsService service = new ProductsService(context2, mapper);

            Response<PaginationResponse<ProductDTO>> response = await service.GetPaginationAsync(new PaginationRequest());

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(3, response.Result.List.Count);
        }

        [TestMethod]
        public async Task GetPagination_ReturnsTwoPages()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();

            context.AddRange(new List<Product>
            {
                new Product { ProductName = "Product 1", ProductDescription = "Description 1", ProductPrice = 10.0m, ProductBarCode = "1234567890123", ProductTax = 5.0, IdProductCategory = 1 },
                new Product { ProductName = "Product 2", ProductDescription = "Description 2", ProductPrice = 20.0m, ProductBarCode = "1234567890124", ProductTax = 10.0, IdProductCategory = 1 },
                new Product { ProductName = "Product 3", ProductDescription = "Description 3", ProductPrice = 30.0m, ProductBarCode = "1234567890125", ProductTax = 15.0, IdProductCategory = 1 }
            });

            await context.SaveChangesAsync();

            // Act
            DataContext context2 = BuildContext(dbName);
            IProductsService service = new ProductsService(context2, mapper);
            PaginationRequest request = new PaginationRequest
            {
                Page = 1,
                RecordsPerPage = 2,
            };

            Response<PaginationResponse<ProductDTO>> response = await service.GetPaginationAsync(request);

            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(2, response.Result.Pages.Count);
            Assert.AreEqual(2, response.Result.List.Count);
        }
        [TestMethod]
        public async Task GetOne_ReturnsNotFound()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<ProductDTO> response = await service.GetOneAsync(1);
            // Assert
            Assert.IsTrue(!response.IsSuccess);
            Assert.IsTrue(response.Message.StartsWith("No existe "));
        }
        [TestMethod]
        public async Task GetOne_ReturnsProductSuccessfully()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();

            Product product = new Product
            {
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = 1
            };

            context.Products.Add(product);
            await context.SaveChangesAsync();
            // Act
            DataContext context2 = BuildContext(dbName);
            IProductsService service = new ProductsService(context2, mapper);
            Response<ProductDTO> response = await service.GetOneAsync(product.Id);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(product.ProductName, response.Result.ProductName);
        }

        [TestMethod]
        public async Task Create_ReturnsSuccess()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();

            ProductDTO dto = new ProductDTO
            {
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = 1
            };

            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<ProductDTO> response = await service.CreateAsync(dto);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            int quantity = await context2.ProductCategories.CountAsync();
            Assert.AreEqual(1, quantity);

            Product product = await context2.Products.FirstOrDefaultAsync();
            Assert.AreEqual(product.ProductName, dto.ProductName);
        }
        [TestMethod]
        public async Task Create_ReturnsFailOnDuplicateName()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();

            Product product = new Product
            {
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = 1
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            ProductDTO dto = new ProductDTO
            {
                ProductName = "Test Product",
                ProductDescription = "Another Description",
                ProductPrice = 200.0m,
                ProductBarCode = "1234567890124",
                ProductTax = 15.0,
                IdProductCategory = 1
            };
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<ProductDTO> response = await service.CreateAsync(dto);
            // Assert
            Assert.IsTrue(!response.IsSuccess);
            Assert.AreEqual("Ya existe un producto con ese nombre.", response.Message);
        }
        [TestMethod]
        public async Task Edit_ReturnsSuccess()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            ProductCategory category = new ProductCategory { ProductCategoryName = "Category 1" };
            context.ProductCategories.Add(category);
            await context.SaveChangesAsync();
            Product product = new Product
            {
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = category.Id
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            var productsAfterInitialSave = await context.Products.ToListAsync();
            ProductDTO dto = new ProductDTO
            {
                Id = product.Id,
                ProductName = "Updated Product",
                ProductDescription = "Updated Description",
                ProductPrice = 150.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 12.0,
                IdProductCategory = category.Id
            };
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<ProductDTO> response = await service.EditAsync(dto);
            var productsAfterUpdate = await context.Products.ToListAsync();
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            var productsInContext2 = await context2.Products.ToListAsync();
            Product updatedProduct = await context2.Products.FindAsync(product.Id);
            Assert.AreEqual("Updated Product", updatedProduct.ProductName);
        }
        [TestMethod]
        public async Task Edit_ReturnsFailOnNonExistentProduct()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            ProductDTO dto = new ProductDTO
            {
                Id = 999,
                ProductName = "Product",
                ProductDescription = "Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = 1
            };
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<ProductDTO> response = await service.EditAsync(dto);
            // Assert
            Assert.IsTrue(!response.IsSuccess);
            Assert.AreEqual("No existe el producto con id 999", response.Message);
        }
        [TestMethod]
        public async Task Delete_ReturnsSuccess()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            context.Add(new ProductCategory { ProductCategoryName = "Category 1" });
            await context.SaveChangesAsync();
            Product product = new Product
            {
                ProductName = "Test Product",
                ProductDescription = "Test Description",
                ProductPrice = 100.0m,
                ProductBarCode = "1234567890123",
                ProductTax = 10.0,
                IdProductCategory = 1
            };
            context.Products.Add(product);
            await context.SaveChangesAsync();
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<object> response = await service.DeleteAsync(product.Id);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            int count = await context2.Products.CountAsync(p => p.Id == product.Id);
            Assert.AreEqual(0, count);
        }
        [TestMethod]
        public async Task Delete_ReturnsFailOnNonExistentProduct()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            // Act
            IProductsService service = new ProductsService(context, mapper);
            Response<object> response = await service.DeleteAsync(999);
            // Assert
            Assert.IsTrue(!response.IsSuccess);
            Assert.AreEqual("El producto con id: 999 no existe", response.Message);
        }
    }
}

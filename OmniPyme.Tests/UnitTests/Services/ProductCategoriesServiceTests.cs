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
    public class ProductCategoriesServiceTests : BaseTests
    {
        [TestMethod]
        public async Task GetPagination_ReturnsAllProductCategories()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            context.AddRange(new List<ProductCategory>
            {
                new ProductCategory { ProductCategoryName = "Category 1" },
                new ProductCategory { ProductCategoryName = "Category 2" },
                new ProductCategory { ProductCategoryName = "Category 3" }
            });
            await context.SaveChangesAsync();
            // Act
            DataContext context2 = BuildContext(dbName);
            IProductCategoriesService service = new ProductCategoriesService(context2, mapper);
            Response<PaginationResponse<ProductCategoryDTO>> response = await service.GetPaginationAsync(new PaginationRequest());
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
            context.AddRange(new List<ProductCategory>
            {
                new ProductCategory { ProductCategoryName = "Category 1" },
                new ProductCategory { ProductCategoryName = "Category 2" },
                new ProductCategory { ProductCategoryName = "Category 3" }
            });
            await context.SaveChangesAsync();
            // Act
            DataContext context2 = BuildContext(dbName);
            IProductCategoriesService service = new ProductCategoriesService(context2, mapper);
            PaginationRequest request = new PaginationRequest
            {
                Page = 1,
                RecordsPerPage = 2,
            };

            Response<PaginationResponse<ProductCategoryDTO>> response = await service.GetPaginationAsync(request);
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
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<ProductCategoryDTO> response = await service.GetOneAsync(1);
            // Assert
            Assert.IsTrue(!response.IsSuccess);
            Assert.IsTrue(response.Message.StartsWith("No existe "));
        }

        [TestMethod]
        public async Task GetOne_ReturnsProductCategorySuccessfully()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            ProductCategory category = new ProductCategory { ProductCategoryName = "Category 1" };

            context.ProductCategories.Add(category);
            await context.SaveChangesAsync();
            // Act
            DataContext context2 = BuildContext(dbName);
            IProductCategoriesService service = new ProductCategoriesService(context2, mapper);
            Response<ProductCategoryDTO> response = await service.GetOneAsync(category.Id);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            Assert.AreEqual(category.ProductCategoryName, response.Result.ProductCategoryName);
        }

        [TestMethod]
        public async Task Create_ReturnsSuccess()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();

            ProductCategoryDTO dto = new ProductCategoryDTO { ProductCategoryName = "Category 1" };

            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<ProductCategoryDTO> response = await service.CreateAsync(dto);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            int quantity = await context2.ProductCategories.CountAsync();
            Assert.AreEqual(1, quantity);

            ProductCategory category = await context2.ProductCategories.FirstOrDefaultAsync();
            Assert.AreEqual(category.ProductCategoryName, dto.ProductCategoryName);
        }
        [TestMethod]
        public async Task Create_ReturnsFailOnDuplicateName()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            ProductCategory category = new ProductCategory { ProductCategoryName = "Category 1" };
            context.ProductCategories.Add(category);
            await context.SaveChangesAsync();
            ProductCategoryDTO dto = new ProductCategoryDTO { ProductCategoryName = "Category 1" };
            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<ProductCategoryDTO> response = await service.CreateAsync(dto);
            // Assert
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("Ya existe una categoría con ese nombre.", response.Message);
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
            var productsAfterInitialSave = await context.ProductCategories.ToListAsync();
            ProductCategoryDTO dto = new ProductCategoryDTO { Id = category.Id, ProductCategoryName = "Updated Category" };
            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<ProductCategoryDTO> response = await service.EditAsync(dto);
            var productsAfterUpdate = await context.ProductCategories.ToListAsync();
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            var productsInContext2 = await context2.ProductCategories.ToListAsync();
            ProductCategory updatedCategory = await context2.ProductCategories.FindAsync(category.Id);
            Assert.AreEqual("Updated Category", updatedCategory.ProductCategoryName);
        }
        [TestMethod]
        public async Task Edit_ReturnsFailOnNotFound()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            ProductCategoryDTO dto = new ProductCategoryDTO { Id = 999, ProductCategoryName = "Updated Category" };
            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<ProductCategoryDTO> response = await service.EditAsync(dto);
            // Assert
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("No existe la categoría con id 999", response.Message);
        }
        [TestMethod]
        public async Task Delete_ReturnsSuccess()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            ProductCategory category = new ProductCategory { ProductCategoryName = "Category 1" };
            context.ProductCategories.Add(category);
            await context.SaveChangesAsync();
            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<object> response = await service.DeleteAsync(category.Id);
            // Assert
            Assert.IsTrue(response.IsSuccess);
            DataContext context2 = BuildContext(dbName);
            int quantity = await context2.ProductCategories.CountAsync();
            Assert.AreEqual(0, quantity);
        }
        [TestMethod]
        public async Task Delete_ReturnsFailOnNotFound()
        {
            // Arrange
            string dbName = Guid.NewGuid().ToString();
            DataContext context = BuildContext(dbName);
            IMapper mapper = ConfigureAutoMapper();
            // Act
            IProductCategoriesService service = new ProductCategoriesService(context, mapper);
            Response<object> response = await service.DeleteAsync(999);
            // Assert
            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual("La categoría con id: 999 no existe", response.Message);
        }
    }
}

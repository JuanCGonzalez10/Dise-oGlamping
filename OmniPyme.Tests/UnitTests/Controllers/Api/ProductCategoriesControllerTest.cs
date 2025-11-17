using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OmniPyme.Web.Controllers.Api;
using OmniPyme.Web.Core;
using OmniPyme.Web.Core.Pagination;
using OmniPyme.Web.DTOs;
using OmniPyme.Web.Services;

namespace OmniPyme.Tests.UnitTests.Controllers.Api;

[TestClass]
public class ProductCategoriesControllerTest : BaseTests
{
    [TestMethod]
    public async Task GetProductsCategories_ReturnsStatus200()
    {
        // Arrange
        Response<PaginationResponse<ProductCategoryDTO>> mockResponse = new()
        {
            IsSuccess = true,
            Result = new PaginationResponse<ProductCategoryDTO>()
        };

        Mock<IProductCategoriesService> productCategoriesMock = new Mock<IProductCategoriesService>();

        productCategoriesMock.Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>())).Returns(Task.FromResult(mockResponse));
        // Act
        ProductCategoriesController controller = new ProductCategoriesController(productCategoriesMock.Object);
        IActionResult actionResult = await controller.GetProductsCategory(new PaginationRequest());

        // Assert
        ObjectResult result = actionResult as ObjectResult;
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);

    }
    [TestMethod]
    public async Task GetProductsCategories_ReturnsStatus400()
    {
        // Arrange
        Response<PaginationResponse<ProductCategoryDTO>> mockResponse = new()
        {
            IsSuccess = false,
            Result = new PaginationResponse<ProductCategoryDTO>()
        };

        Mock<IProductCategoriesService> productCategoriesMock = new Mock<IProductCategoriesService>();

        productCategoriesMock.Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>())).Returns(Task.FromResult(mockResponse));
        // Act
        ProductCategoriesController controller = new ProductCategoriesController(productCategoriesMock.Object);
        IActionResult actionResult = await controller.GetProductsCategory(new PaginationRequest());

        // Assert
        ObjectResult result = actionResult as ObjectResult;
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);

    }
}

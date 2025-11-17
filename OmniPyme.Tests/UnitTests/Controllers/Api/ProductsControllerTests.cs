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
public class ProductsControllerTests : BaseTests
{
    [TestMethod]
    public async Task GetProducts_ReturnsStatus200()
    {
        // Arrange
        Response<PaginationResponse<ProductDTO>> mockResponse = new()
        {
            IsSuccess = true,
            Result = new PaginationResponse<ProductDTO>()
        };
        Mock<IProductsService> productsMock = new Mock<IProductsService>();

        productsMock.Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>())).Returns(Task.FromResult(mockResponse));
        // Act
        ProductsController controller = new ProductsController(productsMock.Object);
        IActionResult actionResult = await controller.GetProducts(new PaginationRequest());
        // Assert
        ObjectResult result = actionResult as ObjectResult;
        Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
    }
    [TestMethod]
    public async Task GetProducts_ReturnsStatus400()
    {
        // Arrange
        Response<PaginationResponse<ProductDTO>> mockResponse = new()
        {
            IsSuccess = false,
            Result = new PaginationResponse<ProductDTO>()
        };
        Mock<IProductsService> productsMock = new Mock<IProductsService>();
        productsMock.Setup(x => x.GetPaginationAsync(It.IsAny<PaginationRequest>())).Returns(Task.FromResult(mockResponse));

        // Act
        ProductsController controller = new ProductsController(productsMock.Object);
        IActionResult actionResult = await controller.GetProducts(new PaginationRequest());

        // Assert
        ObjectResult result = actionResult as ObjectResult;
        Assert.AreEqual(StatusCodes.Status400BadRequest, result.StatusCode);
    }
}

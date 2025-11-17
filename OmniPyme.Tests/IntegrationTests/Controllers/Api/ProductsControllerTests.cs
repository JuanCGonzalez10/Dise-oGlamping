using System.Net;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using OmniPyme.Web.Core;
using OmniPyme.Web.DTOs;

namespace OmniPyme.Tests.IntegrationTests.Controllers.Api;

[TestClass]
public class ProductsControllerTests : BaseTests
{
    private static readonly string url = "api/Products";
    private async Task<string> GetJwtTokenAsync(HttpClient client, string email, string password)
    {
        LoginDTO loginDto = new LoginDTO
        {
            Email = email,
            Password = password
        };

        StringContent loginContent = new StringContent(JsonConvert.SerializeObject(loginDto), Encoding.UTF8, "application/json");
        HttpResponseMessage loginResponse = await client.PostAsync("/api/ApiAccount/Login", loginContent);

        loginResponse.EnsureSuccessStatusCode();

        string loginJson = await loginResponse.Content.ReadAsStringAsync();
        Response<UserTokenDTO> loginData = JsonConvert.DeserializeObject<Response<UserTokenDTO>>(loginJson);

        return loginData.Result.Token;
    }
    [TestMethod]
    public async Task GetProductCategories_Returns_Unauthorized_Without_Token()
    {
        // Arrange
        string dbName = Guid.NewGuid().ToString();
        WebApplicationFactory<Program> factory = BuildWebApplicationFactory(dbName);

        // Act
        HttpClient client = factory.CreateClient();
        HttpResponseMessage response = await client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [TestMethod]
    public async Task GetProductCategories_WithValidToken_Returns200()
    {
        // Arrange
        string dbName = Guid.NewGuid().ToString();
        WebApplicationFactory<Program> factory = BuildWebApplicationFactory(dbName);
        HttpClient client = factory.CreateClient();

        string token = await GetJwtTokenAsync(client, "inventario@yopmail.com", "1234");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
    }

    [TestMethod]
    public async Task GetProductCategories_WithValidToken_Returns403()
    {
        // Arrange
        string dbName = Guid.NewGuid().ToString();
        WebApplicationFactory<Program> factory = BuildWebApplicationFactory(dbName);
        HttpClient client = factory.CreateClient();

        string token = await GetJwtTokenAsync(client, "vendedor@yopmail.com", "1234");

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        HttpResponseMessage response = await client.GetAsync(url);

        // Assert
        Assert.AreEqual(HttpStatusCode.Forbidden, response.StatusCode);
    }
}

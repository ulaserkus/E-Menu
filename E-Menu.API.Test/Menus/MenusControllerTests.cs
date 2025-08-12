using Microsoft.AspNetCore.Mvc.Testing;
using Shared.Kernel;
using Shared.Kernel.Dtos;
using Shared.Kernel.Requests;
using System.Net;
using System.Text;
using System.Text.Json;

namespace E_Menu.API.Test.Menus;

public class MenusControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private const string BaseUrl = "/api/menus/";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };

    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public MenusControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private static StringContent CreateJsonContent<T>(T body)
    {
        return new StringContent(JsonSerializer.Serialize(body, JsonSerializerOptions), Encoding.UTF8, "application/json");
    }

    private static HttpRequestMessage CreateHttpRequest<T>(string endpoint, T body)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, BaseUrl + endpoint)
        {
            Content = CreateJsonContent(body)
        };
        return request;
    }


    [Theory]
    [InlineData("32d184af-fe34-f011-8c4e-6045bde16962")]
    public async Task GetProductCategoriesByMenu(Guid menuId)
    {
        // Arrange
        var body = new GetMenuProductCategoriesRequest()
        {
            MenuId = menuId
        };
        var request = CreateHttpRequest("Categories", body);
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Result<IEnumerable<ProductCategoryDto>>>(responseBody, JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }


    [Theory]
    [InlineData("32d184af-fe34-f011-8c4e-6045bde16962")]
    public async Task GetMenuItems_ReturnsOk(Guid menuId)
    {
        // Arrange
        var body = new GetMenuWithItemsRequest()
        {
            MenuId = menuId
        };
        var request = CreateHttpRequest("Items", body);
        // Act
        var response = await _client.SendAsync(request);
        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Result<MenuWithItemsDto>>(responseBody, JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }


    [Fact]
    public async Task GetAllMenus_ReturnsOk()
    {
        // Arrange
        var body = new GetAllMenuItemsRequest();
        var request = CreateHttpRequest("All", body);

        // Act
        var response = await _client.SendAsync(request);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var responseBody = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<Result<IEnumerable<MenuDto>>>(responseBody, JsonSerializerOptions);
        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }
}
using System.Net;
using System.Text;
using System.Text.Json;
using EntityFrameworkDapperApp.Core.Entities.DTOs;
using EntityFrameworkDapperApp.Tests.Helpers;
using EntityFrameworkDapperApp.Tests.IntegrationTests.Fixtures;

namespace EntityFrameworkDapperApp.Tests.IntegrationTests;

[Collection("SharedContext")]
public class BookIntegrationTests
{
    private readonly JsonSerializerOptions _serializerOptions;
    private readonly HttpClient _httpClient;
    private const string BasePath = "/api/v2/books";

    public BookIntegrationTests(CustomWebApplicationFactory<Program> factory)
    {
        _httpClient = factory.CreateClient();

        _serializerOptions = new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    #region GetAll

    [Fact]
    public async Task GeAll_ShouldReturnAllItems()
    {
        var response = await _httpClient.GetAsync($"{BasePath}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<List<BookDto>>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(payloadObject?.Count >= DbHelper.Books.Count);
    }

    #endregion

    #region GeById

    [Theory]
    [MemberData(nameof(ItemIds))]
    public async Task GeById_ShouldReturnOnlyOneItem(Guid itemId)
    {
        var existingItem = DbHelper.Books.SingleOrDefault(x => x.Id == itemId);

        var response = await _httpClient.GetAsync($"{BasePath}/{existingItem?.Id}");
        var payloadString = await response.Content.ReadAsStringAsync();
        var payloadObject = JsonSerializer.Deserialize<BookDto>(payloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(existingItem?.Title, payloadObject?.Title);
        Assert.Equal(existingItem?.PublishedOn.Date, payloadObject?.PublishedOn.Date);
    }

    [Fact]
    public async Task GeById_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();

        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GeById_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";

        var response = await _httpClient.GetAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Create

    [Fact]
    public async Task Create_ShouldCreateAnItem()
    {
        var newItem = new
        {
            Title = "New Book 01",
            PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
        };
        var payload = JsonSerializer.Serialize(newItem, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var locationHeader = response.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last().Split('?').First();

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotEqual(Guid.Empty, new Guid(newItemId!));
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsBase))]
    [MemberData(nameof(MissingRequiredFieldsForCreating))]
    public async Task Create_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(InvalidFields))]
    public async Task Create_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{BasePath}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion

    #region Update

    [Fact]
    public async Task Update_ShouldUpdateAnItem()
    {
        // Create a new item
        var newItem = new
        {
            Title = "This is a new book 01",
            PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
        };
        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);

        var locationHeader = newItemResponse.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last().Split('?').First();

        // Update the created item
        var itemToUpdate = new
        {
            Id = newItemId,
            Title = "This is a new book 01 (updated)",
            PublishedOn = new DateTime(2022, 01, 24).ToUniversalTime(),
        };
        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BasePath}/{newItemId}", httpContent);

        // Ensure the item has been changed getting the item from the DB
        var updatedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemId}");
        var updatedItemPayloadString = await updatedItemResponse.Content.ReadAsStringAsync();
        var updatedItemPayloadObject =
            JsonSerializer.Deserialize<BookDto>(updatedItemPayloadString, _serializerOptions);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.NotEqual(Guid.Empty, updatedItemPayloadObject?.Id);
        Assert.Equal(itemToUpdate?.Title, updatedItemPayloadObject?.Title);
        Assert.Equal(itemToUpdate?.PublishedOn.Date, updatedItemPayloadObject?.PublishedOn.Date);
    }

    [Fact]
    public async Task Update_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var itemToUpdate = new
        {
            Id = itemId,
            Title = "This is a new book 01",
            PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
        };

        var payload = JsonSerializer.Serialize(itemToUpdate, _serializerOptions);
        var httpContent = new StringContent(payload, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Theory]
    [MemberData(nameof(MissingRequiredFieldsBase))]
    [MemberData(nameof(MissingRequiredFieldsForUpdating))]
    public async Task Update_ShouldReturnBadRequestWhenMissingRequiredFields(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    [Theory]
    [MemberData(nameof(InvalidFields))]
    public async Task Update_ShouldReturnBadRequestWhenFieldsAreInvalid(string[] expectedCollection,
        object payloadObject)
    {
        var payload = JsonSerializer.Serialize(payloadObject, _serializerOptions);
        HttpContent httpContent = new StringContent(payload, Encoding.UTF8, "application/json");

        var itemId = new Guid();
        var response = await _httpClient.PutAsync($"{BasePath}/{itemId.ToString()}", httpContent);
        var payloadString = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.All(expectedCollection, expected => Assert.Contains(expected, payloadString));
    }

    #endregion

    #region Remove

    [Fact]
    public async Task Remove_ShouldRemoveOnlyOneItem()
    {
        // Create a new item
        var newItem = new
        {
            Title = "This is a new book 01",
            PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
        };
        var newItemPayload = JsonSerializer.Serialize(newItem, _serializerOptions);
        var newItemHttpContent = new StringContent(newItemPayload, Encoding.UTF8, "application/json");
        var newItemResponse = await _httpClient.PostAsync($"{BasePath}", newItemHttpContent);

        var locationHeader = newItemResponse.Headers.GetValues("Location").FirstOrDefault();
        var newItemId = locationHeader?.Split('/').Last();

        // Remove the created item
        var response = await _httpClient.DeleteAsync($"{BasePath}/{newItemId}");

        // Ensure the item has been deleted trying to get the item from the DB
        var deletedItemResponse = await _httpClient.GetAsync($"{BasePath}/{newItemId}");

        Assert.Equal(HttpStatusCode.Created, newItemResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.Equal(HttpStatusCode.NotFound, deletedItemResponse.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnNotFoundWhenIdNotExists()
    {
        var itemId = new Guid();
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Remove_ShouldReturnBadRequestWhenIdIsNotValid()
    {
        const string itemId = "not-valid-id";
        var response = await _httpClient.DeleteAsync($"{BasePath}/{itemId}");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    public static TheoryData<string[], object> MissingRequiredFieldsBase => new()
    {
        {
            new[]
            {
                "The Title field is required.",
                "The PublishedOn field is required."
            },
            new { }
        },
        {
            new[] { "The Title field is required." }, new
            {
                PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
            }
        },
        {
            new[] { "The PublishedOn field is required." }, new
            {
                Title = "New Book 01",
            }
        }
    };

    public static TheoryData<string[], object> MissingRequiredFieldsForCreating => new()
    {
    };

    public static TheoryData<string[], object> MissingRequiredFieldsForUpdating => new()
    {
        {
            new[]
            {
                "The Id field is required.",
                "The Title field is required.",
                "The PublishedOn field is required."
            },
            new { }
        },
        {
            new[] { "The Id field is required." }, new
            {
                Title = "New Book 01",
                PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
            }
        },
    };

    public static TheoryData<string[], object> InvalidFields => new()
    {
        {
            new[] { "The Title field is invalid." }, new
            {
                Id = DbHelper.BookId1,
                Title = "New Book 01 *",
                PublishedOn = new DateTime(2022, 01, 12).ToUniversalTime(),
            }
        },
    };

    public static TheoryData<Guid> ItemIds => new()
    {
        { DbHelper.BookId1 },
        { DbHelper.BookId2 },
        { DbHelper.BookId3 },
        { DbHelper.BookId4 },
        { DbHelper.BookId5 },
    };
}
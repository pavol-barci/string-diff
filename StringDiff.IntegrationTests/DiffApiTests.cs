using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Newtonsoft.Json;
using StringDiff.Contracts;
using Xunit;

namespace StringDiff.IntegrationTests;

public class DiffApiTests
{
    private readonly HttpClient _client = new()
    {
        BaseAddress = new Uri("http://localhost:5105"),
    };
    
    [Fact]
    public async Task DiffV1Get_ModelNotFound_ReturnsNotFoundResponse()
    {
        var id = Guid.NewGuid();
        var response = await _client.GetAsync($"diff/v1/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var contentString = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contentString);

        errorResponse.Should().NotBeNull();
        errorResponse.ValidationErrors.Should().BeNull();
        errorResponse.Message.Should().Be($"Model with id {id} does not exists.");
    }

    [Theory]
    [InlineData("left")]
    [InlineData("right")]
    public async Task DiffV1_InvalidRequest_ReturnsBadRequestResponse(string path)
    {
        var request = new { };
        var id = Guid.NewGuid();
        
        var response = await _client.PutAsync($"diff/v1/{id}/{path}", JsonContent.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var contentString = await response.Content.ReadAsStringAsync();
        var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(contentString);

        errorResponse.Should().NotBeNull();
        errorResponse.Message.Should().Be("Validation of request failed.");
        errorResponse.ValidationErrors.Should().NotBeNullOrEmpty();
        errorResponse.ValidationErrors.Should().Contain(o => o.Property == "diffRequest");
    }

    [Theory]
    [InlineData("left")]
    [InlineData("right")]
    public async Task DiffV1_CreatesNewModel_ReturnsCreatedResponse(string path)
    {
        var id = Guid.NewGuid();
        var request = new
        {
            input = Convert.ToBase64String("stkkjg"u8.ToArray())
        };

        var response = await _client.PutAsync($"diff/v1/{id}/{path}", JsonContent.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Theory]
    [InlineData("left")]
    [InlineData("right")]
    public async Task DiffV1_UpdatesExistingModel_ReturnsOkResponse(string path)
    {
        var id = Guid.NewGuid();
        var request = new
        {
            input = Convert.ToBase64String("stkkjg"u8.ToArray())
        };
        await _client.PutAsync($"diff/v1/{id}/{path}", JsonContent.Create(request));
        
        
        var response = await _client.PutAsync($"diff/v1/{id}/{path}", JsonContent.Create(request));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("left")]
    [InlineData("right")]
    public async Task DiffV1_GetNotFinished_ReturnsOkResponseWithResult(string path)
    {
        var id = Guid.NewGuid();
        var request = new
        {
            input = Convert.ToBase64String("stkkjg"u8.ToArray())
        };
        await _client.PutAsync($"diff/v1/{id}/{path}", JsonContent.Create(request));
        
        var response = await _client.GetAsync($"diff/v1/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contentString = await response.Content.ReadAsStringAsync();
        var successResponse = JsonConvert.DeserializeObject<DiffResultResponse>(contentString);

        successResponse.Should().NotBeNull();
        successResponse.Result.Should().Be(DiffResultResponseType.NotFinished);
        successResponse.Difference.Should().BeNull();
    }

    [Fact]
    public async Task DiffV1_GetFinished_ReturnsOkResponseWithResult()
    {
        var id = Guid.NewGuid();
        var request = new
        {
            input = Convert.ToBase64String("string"u8.ToArray())
        };
        await _client.PutAsync($"diff/v1/{id}/left", JsonContent.Create(request));
        await _client.PutAsync($"diff/v1/{id}/right", JsonContent.Create(request));
        
        var response = await _client.GetAsync($"diff/v1/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contentString = await response.Content.ReadAsStringAsync();
        var successResponse = JsonConvert.DeserializeObject<DiffResultResponse>(contentString);

        successResponse.Should().NotBeNull();
        successResponse.Result.Should().Be(DiffResultResponseType.Equal);
        successResponse.Difference.Should().BeNull();
    }

    [Fact]
    public async Task DiffV1_GetFinishedThenUpdateAgain_ReturnsOkResponseWithResult()
    {
        var id = Guid.NewGuid();
        var request = new
        {
            input = Convert.ToBase64String("string"u8.ToArray())
        };
        await _client.PutAsync($"diff/v1/{id}/left", JsonContent.Create(request));
        await _client.PutAsync($"diff/v1/{id}/right", JsonContent.Create(request));
        
        var response = await _client.GetAsync($"diff/v1/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var contentString = await response.Content.ReadAsStringAsync();
        var successResponse = JsonConvert.DeserializeObject<DiffResultResponse>(contentString);

        successResponse.Should().NotBeNull();
        successResponse.Result.Should().Be(DiffResultResponseType.Equal);
        successResponse.Difference.Should().BeNull();
        
        request = new
        {
            input = Convert.ToBase64String("stkkjg"u8.ToArray())
        };
        
        await _client.PutAsync($"diff/v1/{id}/right", JsonContent.Create(request));
        
        response = await _client.GetAsync($"diff/v1/{id}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        contentString = await response.Content.ReadAsStringAsync();
        successResponse = JsonConvert.DeserializeObject<DiffResultResponse>(contentString);

        successResponse.Should().NotBeNull();
        successResponse.Result.Should().Be(DiffResultResponseType.NotEquals);
        successResponse.Difference.Should().NotBeNull();
        successResponse.Difference.Offset.Should().Be(2);
        successResponse.Difference.Length.Should().Be(4);
    }
}
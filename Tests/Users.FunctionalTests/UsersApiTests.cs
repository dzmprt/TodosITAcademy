using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Tests;
using Core.Tests.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Users.Application.Dtos;
using Users.Application.Handlers.Commands.CreateUser;
using Users.Application.Handlers.Commands.DeleteUser;
using Users.Application.Handlers.Commands.UpdateUser;
using Users.Application.Handlers.Commands.UpdateUserPassword;
using Users.Application.Handlers.Queries.GetUser;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Users.FunctionalTests;

public class UsersApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const string adminToken =
        "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMjUyMTc4LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0.Cs2O0VLJFtq_jEsT4DPzhIkjRhfwi9SJsW5ojMJd4Ps";

    public UsersApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Theory]
    [FixtureInlineAutoData(null, null)]
    [FixtureInlineAutoData(null, 1)]
    [FixtureInlineAutoData(1, null)]
    [FixtureInlineAutoData(1, 1)]
    public async Task Get_Users_ReturnSuccessAndCorrectContentType(int? limit, int? offset)
    {
        // Arrange
        var client = _factory.CreateClient();

        var query = new Dictionary<string, string?>
        {
            ["limit"] = limit?.ToString(),
            ["offset"] = offset?.ToString(),
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/UM/api/v1/Users", query)))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [FixtureInlineAutoData("17593112-CD8D-4C96-893B-F53C8CC31CDA")]
    public async Task Get_UserById_ReturnSuccessAndCorrectContentType(string id)
    {
        // Arrange
        var client = _factory.CreateClient();

        var query = new GetUserQuery()
        {
            Id = id
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, $"/UM/api/v1/Users/{query.Id}"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(responseObject!.ApplicationUserId.ToString(), id, true);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [FixtureInlineAutoData("Client")]
    public async Task Get_CountUsers_ReturnCountUsersAndCorrectContentType(string freeText)
    {
        // Arrange
        var client = _factory.CreateClient();

        var query = new Dictionary<string, string?>
        {
            ["freeText"] = freeText,
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/UM/api/v1/Users/Count", query)))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Fact]
    public async Task Create_User_ReturnCreatedAndCorrectContentType()
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Login = "Login",
            Password = "12345678"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/UM/api/v1/Users"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            Assert.Equal(responseObject!.Login, command.Login);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [FixtureInlineAutoData("35F37340-F9E5-4118-B949-08DC51CC57B7")]
    public async Task UpdatePassword_User_ReturnUpdatedPasswordAndStatusCodeOK(string id)
    {
        // Arrange
        var commandPayload = new UpdateUserPasswordPayload()
        {
            Password = "1234567890"
        };
        var command = new UpdateUserPasswordCommand()
        {
            UserId = id,
            Password = commandPayload.Password,
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/UM/api/v1/Users/{id}/Password"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
        }
    }

    [Theory]
    [FixtureInlineAutoData("17593112-cd8d-4c96-893b-f53c8cc31cda")]
    public async Task UpdateUser_ReturnUpdatedLoginAndCorrectContentType(string id)
    {
        // Arrange
        var commandPayload = new UpdateUserPayload()
        {
            Login = "Login",
        };
        var command = new UpdateUserCommand()
        {
            Id = id,
            Login = commandPayload.Login,
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/UM/api/v1/Users/{id}"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal(responseObject!.ApplicationUserId.ToString(), command.Id);
            Assert.Equal(responseObject!.Login, command.Login);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Theory]
    [FixtureInlineAutoData("17593112-CD8D-4C96-893B-F53C8CC31CDA")]
    public async Task DeleteUser_ReturnStatusCodeOK(string id)
    {
        // Arrange
        var command = new DeleteUserCommand() { Id = id };

        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/UM/api/v1/Users/{id}"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
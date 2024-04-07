using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Tests.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Users.Application.Dtos;
using Users.Application.Handlers.Commands.CreateUser;
using Users.Application.Handlers.Commands.DeleteUser;
using Users.Application.Handlers.Commands.UpdateUser;
using Users.Application.Handlers.Commands.UpdateUserPassword;
using Users.Application.Handlers.Queries.GetUser;
using Core.Tests;
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

    #region Queries

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

    [Fact]
    public async Task Get_UserById_ReturnSuccessAndCorrectContentTypeWithId()
    {
        // Arrange
        var client = _factory.CreateClient();

        var query = new GetUserQuery()
        {
            Id = "17593112-CD8D-4C96-893B-F53C8CC31CDA"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, "/UM/api/v1/Users/" + query.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(query.Id, responseObject!.ApplicationUserId.ToString(), true);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Fact]
    public async Task Get_UsersCount_ReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        var query = new Dictionary<string, string?>
        {
            ["FreeText"] = "Client"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/UM/api/v1/Users/Count", query)))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
    #endregion

    #region Commands

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

    [Fact]
    public async Task Put_User_ReturnSuccessAndCorrectContentTypeWithNewLogin()
    {
        // Arrange
        var client = _factory.CreateClient();

        var command = new UpdateUserCommand()
        {
            Id = "17593112-CD8D-4C96-893B-F53C8CC31CDA",
            Login = "NewLoginTest"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Put, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(command.Id, responseObject!.ApplicationUserId.ToString(), true);
            Assert.Equal(command.Login, responseObject!.Login);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Fact]
    public async Task Patch_UserPassword_ReturnStatusCode200()
    {
        // Arrange
        var client = _factory.CreateClient();

        var command = new UpdateUserPasswordCommand()
        {
            UserId = "17593112-CD8D-4C96-893B-F53C8CC31CDA",
            Password = "NewPasswordTest"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Patch, $"/UM/api/v1/Users/{command.UserId}/Password"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task Delete_User_ReturnStatusCode200()
    {
        // Arrange
        var client = _factory.CreateClient();

        var command = new DeleteUserCommand()
        {
            Id = "17593112-CD8D-4C96-893B-F53C8CC31CDA"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Delete, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
    #endregion
}
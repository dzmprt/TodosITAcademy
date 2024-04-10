using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Tests.Attributes;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Moq;
using Users.Application.Dtos;
using Users.Application.Handlers.Commands.CreateUser;
using Users.Application.Handlers.Commands.UpdateUser;
using Users.Application.Handlers.Commands.UpdateUserPassword;
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

    [Fact]
    public async Task Get_User_ReturnsUser()
    {
        // Arrange
        var userId = "17593112-CD8D-4C96-893B-F53C8CC31CDA";
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, $"/UM/api/v1/Users/{userId}"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            var user = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(user);
            Assert.Equal(Guid.Parse(userId), user.ApplicationUserId);
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


    [Fact]
    public async Task Patch_User_Password_ReturnSuccess()
    {
        // Arrange
        var userId = "35F37340-F9E5-4118-B949-08DC51CC57B7";
        var newPassword = "new_password";
        var payload = new UpdateUserPasswordPayload { Password = newPassword };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/UM/api/v1/Users/{userId}/Password"))
        {
            requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task Get_UsersCount_ReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage =
            new HttpRequestMessage(HttpMethod.Get, "/UM/api/v1/Users/Count"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            // Assert
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var responseJson = await response.Content.ReadAsStringAsync();
            var count = JsonSerializer.Deserialize<int>(responseJson);

            // Assert
            Assert.True(count >= 0);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    //[Fact]
    //public async Task Delete_User_ReturnsNoContent()
    //{
    //    // Arrange
    //    var userId = "2B4945AB-97A7-49C8-098B-08DC5356FBAA";
    //    var client = _factory.CreateClient();

    //    // Act
    //    using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/UM/api/v1/Users/{userId}"))
    //    {
    //        requestMessage.Headers.Authorization =
    //            new AuthenticationHeaderValue("Bearer", adminToken);

    //        var response = await client.SendAsync(requestMessage);

    //        // Assert
    //        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    //    }
    //}

    [Fact]
    public async Task Put_User_ReturnsSuccessAndCorrectContentType()
    {
        // Arrange
        var userId = "17593112-CD8D-4C96-893B-F53C8CC31CDA";
        var newLogin = "NewLogin";
        var payload = new UpdateUserPayload { Login = newLogin };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/UM/api/v1/Users/{userId}"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());

            var responseJson = await response.Content.ReadAsStringAsync();
            var updatedUser = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            Assert.NotNull(updatedUser);
            Assert.Equal(userId.ToLower(), updatedUser.ApplicationUserId.ToString().ToLower());
            Assert.Equal(newLogin, updatedUser.Login);
        }
    }
}




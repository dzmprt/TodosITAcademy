using System.ComponentModel.Design;
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
using Users.Application.Handlers.Queries.GetUsersCount;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Users.FunctionalTests;

public class UsersApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    /*
{
  "jwtToken": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVGVzdENsaWVudDEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3NTkzMTEyLWNkOGQtNGM5Ni04OTNiLWY1M2M4Y2MzMWNkYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkNsaWVudCIsImV4cCI6MTc0MjU5OTMzNiwiaXNzIjoiVG9kb3MiLCJhdWQiOiJUb2RvcyJ9.-_p-dqK_yPvr3WycFdCDgT1VjOOEDCGrEb4Q2gicckg",
  "refreshToken": "17593112-cd8d-4c96-893b-f53c8cc31cda",
  "expires": "2025-03-21T23:22:16.809713Z"
}     
     
     */

    private const string adminToken =
        "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVGVzdENsaWVudDEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3NTkzMTEyLWNkOGQtNGM5Ni04OTNiLWY1M2M4Y2MzMWNkYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkNsaWVudCIsImV4cCI6MTc0MjY2MjU3NCwiaXNzIjoiVG9kb3MiLCJhdWQiOiJUb2RvcyJ9.Ybed_55LO5ktT8nVnWzMDoiy5fL9SFFcPdFcXcRJJ1Y";

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
    public async Task Get_User_ReturnSuccessAndCorrectContentType()
    {
        // Arrange
        var client = _factory.CreateClient();
        var query = new GetUserQuery
        {
            Id = "17593112-cd8d-4c96-893b-f53c8cc31cda"
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, $"/UM/api/v1/Users/{query.Id}"))
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
    [FixtureInlineAutoData("TestClient1", 1)]
//    [FixtureInlineAutoData("TestClient" , 2)]
//    [FixtureInlineAutoData("TestClient3", 0)]
    public async Task Get_UserCount_ReturnSuccessAndQuantityAndCorrectContentType(string userName, int qt)
    {
        // Arrange
        var client = _factory.CreateClient();
        var query = new Dictionary<string, string?>
        {
            ["FreeText"] = userName,
        };

        // Act
        using (var requestMessage =
               new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/UM/api/v1/Users/Count", query)))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await client.SendAsync(requestMessage);
            var responseString = await response.Content.ReadAsStringAsync();
            var responseQt = int.Parse(responseString);
            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal(responseQt, qt);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
    [Theory]
    [FixtureInlineAutoData("123", "12345678")]
    [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890",
        "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]
    public async Task Create_User_ReturnCreatedAndCorrectContentType(string login, string password)
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Login = login,
            Password = password
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
    [FixtureInlineAutoData("17593112-cd8d-4c96-893b-f53c8cc31cda", "123")]
    [FixtureInlineAutoData("17593112-cd8d-4c96-893b-f53c8cc31cda",
        "12345678901234567890123456789012345678901234567890")]
    public async Task Edit_User_ReturnOkAndCorrectContentType(string id, string newLogin)
    {
        // Arrange
        var command = new UpdateUserCommand()
        {
            Id = id,
            Login = newLogin
            
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
            Assert.Equal(responseObject!.Login, command.Login);
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Fact]
    public async Task Delete_User_ReturnOk()
    {
        // Arrange
        var command = new DeleteUserCommand()
        {
            Id = "17593112-cd8d-4c96-893b-f53c8cc31cda",
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/UM/api/v1/Users/{command.Id}"))
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

    [Theory]
    [FixtureInlineAutoData("17593112-cd8d-4c96-893b-f53c8cc31cda", "12345678")]
    [FixtureInlineAutoData("17593112-cd8d-4c96-893b-f53c8cc31cda",
        "1234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890")]

    public async Task Update_UserPassword_ReturnOk(string userId, string newPassword)
    {
        // Arrange
        var command = new UpdateUserPasswordCommand()
        {
            UserId = userId,
            Password = newPassword
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/UM/api/v1/Users/{command.UserId}/password"))
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
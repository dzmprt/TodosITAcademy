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
using Users.Application.Handlers.Queries.GetUsers;
using Users.Application.Handlers.Queries.GetUsersCount;
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
    #region --Positive--
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
    public async Task Get_User_ReturnOk()
    {
        var command = new GetUserQuery()
        {
            Id = "f596e3a5-17af-4e8c-7f9b-08dc587db203",
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }


    [Fact]
    public async Task GetCount_User_ReturnOk()
    {
        var command = new GetUsersCountQuery()
        {
            FreeText = "test"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/UM/api/v1/Users/"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
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
    public async Task Update_User_ReturnOk()
    {
        var command = new UpdateUserCommand()
        {
            Id = "f596e3a5-17af-4e8c-7f9b-08dc587db203",
            Login = "test login"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task UpdatePassword_User_ReturnOk()
    {
        var command = new UpdateUserPasswordCommand()
        {
            UserId = "397833e1-f069-4b89-7fa2-08dc587db203",
            Password = "1234567890"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/UM/api/v1/Users/" + command.UserId + "/Password"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task Delete_User_ReturnOk() //!!!
    {
        // Arrange
        var command = new DeleteUserCommand()
        {
            Id = "f596e3a5-17af-4e8c-7f9b-08dc587db202"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
    #endregion

    #region --Negative--
    [Theory]
    [FixtureInlineAutoData(null, -1)]
    [FixtureInlineAutoData(-1, null)]
    [FixtureInlineAutoData(-1, 1)]
    public async Task Get_Users_InvalidRequest(int? limit, int? offset)
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
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task Get_User_ReturnNotFound_WhenNoUser()
    {
        var command = new GetUserQuery()
        {
            Id = "0c0b2152-6955-47a8-783a-08dc5876efdb",
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    
    [Fact]
    public async Task Create_User_ReturnNotValid_WhenIdExists()
    {
        // Arrange
        var command = new CreateUserCommand()
        {
            Login = "admin",
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

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }

    [Fact]
    public async Task Update_User_ReturnNotFound_WhenNotFound()
    {
        var command = new UpdateUserCommand()
        {
            Id = "a2939f2f-4b99-4797-783b-08dc5876efdb",
            Login = "test login"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    [Fact]
    public async Task UpdatePassword_ReturnNotFound_WhenNotFound()
    {
        var command = new UpdateUserPasswordCommand()
        {
            UserId = "397833e1-f069-4b89-7fa2-08dc587db202",
            Password = "1234567890"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, "/UM/api/v1/Users/" + command.UserId + "/Password"))
        {
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }

    
    [Fact]
    public async Task Delete_User_ReturnNotFound_WhenNotFound() //!!!
    {
        // Arrange
        var command = new DeleteUserCommand()
        {
            Id = "f596e3a5-17af-4e8c-7f9b-08dc587db202"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, "/UM/api/v1/Users/" + command.Id))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

            var response = await client.SendAsync(requestMessage);

            var responseJson = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
    #endregion
}
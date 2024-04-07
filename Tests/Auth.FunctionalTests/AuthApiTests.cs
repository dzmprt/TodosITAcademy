using Core.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using System.Text.Json;
using Xunit;
using Auth.Application.Handlers.Commands.CreateJwtToken;
using Auth.Application.Handlers.Commands.CreateJwtTokenByRefreshToken;
using System.Net.Http.Headers;
using Auth.Application.Handlers.Queries.GetCurrentUser;

namespace Auth.FunctionalTests;

public class AuthApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const string adminToken =
        "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMjUyMTc4LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0.Cs2O0VLJFtq_jEsT4DPzhIkjRhfwi9SJsW5ojMJd4Ps";

    public AuthApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    #region Queries

    [Fact]
    public async Task Get_CurrentUser_ReturnCreatedAndCorrectContentType()
    {
        // Arrange
        var query = new GetCurrentUserQuery();
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/auth/api/v1/Users/Current"))
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
    public async Task Create_JwtToken_ReturnCreatedAndCorrectContentType()
    {
        // Arrange
        var command = new CreateJwtTokenCommand()
        {
            Login = "Admin",
            Password = "12345678"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/LoginJwt"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

            var response = await client.SendAsync(requestMessage);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("application/json; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }

    [Fact]
    public async Task Create_JwtTokenByRefreshToken_ReturnCreatedAndCorrectContentType()
    {
        // Arrange
        var command = new CreateJwtTokenByRefreshTokenCommand()
        {
            RefreshToken = "972DA343-EB6B-4DC1-7C41-08DC5739EBB8"
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/RefreshJwt"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

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

}

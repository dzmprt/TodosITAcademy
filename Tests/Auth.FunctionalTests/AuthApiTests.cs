using Auth.Application.Dtos;
using Auth.Application.Handlers.Commands.CreateJwtToken;
using Auth.Application.Handlers.Commands.CreateJwtTokenByRefreshToken;
using Core.Tests;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;
using System.Text;
using System.Net.Http.Headers;

namespace Auth.FunctionalTests
{
    public class AuthApiTests: IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private const string adminToken =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMjUyMTc4LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0.Cs2O0VLJFtq_jEsT4DPzhIkjRhfwi9SJsW5ojMJd4Ps";

        public AuthApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Create_JwtToken_ReturnCreatedJwtTokenAndCorrectContentType()
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

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JwtTokenDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.IsType<JwtTokenDto>(responseObject);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task Refresh_JwtToken_ReturnRefreshedTokenAndCorrectContentType()
        {
            // Arrange
            var command = new CreateJwtTokenByRefreshTokenCommand()
            {
                RefreshToken = "35F37340-F9E5-4118-B949-08DC51CC57B7"
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/RefreshJwt"))
            {
                requestMessage.Headers.Authorization = 
                    new AuthenticationHeaderValue("Bearer", adminToken);

                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                var responseJson = await response.Content.ReadAsStringAsync();

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task Get_CurrentUser_ReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage =
                   new HttpRequestMessage(HttpMethod.Get, "/auth/api/v1/Users/Current"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", adminToken);

                var response = await client.SendAsync(requestMessage);

                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<GetUserDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.IsType<GetUserDto>(responseObject);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
    }
}
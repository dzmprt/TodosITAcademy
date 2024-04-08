using Auth.Application.Handlers.Commands.CreateJwtToken;
using Auth.Application.Handlers.Commands.CreateJwtTokenByRefreshToken;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Auth.Application.Dtos;
using Xunit;

namespace Auth.FunctionalTests
{
    public class AuthApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private const string refreshToken = "35F37340-F9E5-4118-B949-08DC51CC57B7";

        private const string adminToken =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyNTgwOTk5LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0._CBbJeuZBDy6R7aYm7H0agZJ9Fr6WTF_1uB4Ggi0U1I";
        private const string existLogin = "Admin";
        private const string notExistLogin = "$Admin$";
        private const string password = "12345678";

        public AuthApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        [Fact]
        public async Task Create_JWTToken_ReturnCreatedAndCorrectContentType()
        {
            // Arrange
            var command = new CreateJwtTokenCommand()
            {
                Login = existLogin,
                Password = password
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "auth/api/v1/LoginJwt"))
            {

                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                
                var response = await client.SendAsync(requestMessage);

                //var responseJson = await response.Content.ReadAsStringAsync();
               
                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
        [Fact]
        public async Task Create_JWTToken_ReturnNotFoundAndCorrectContentType()
        {
            // Arrange
            var command = new CreateJwtTokenCommand()
            {
                Login = notExistLogin,
                Password = password
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "auth/api/v1/LoginJwt"))
            {

                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                
                var response = await client.SendAsync(requestMessage);

                //var responseJson = await response.Content.ReadAsStringAsync();
               
                // Assert
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
                Assert.Equal("application/json",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task Create_JWTTokenByRefreshToken_ReturnCreatedAndCorrectContentType()
        {
            // Arrange
            var command = new CreateJwtTokenByRefreshTokenCommand()
            {
                RefreshToken = refreshToken,
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
        public async Task Get_CurrentUser_ReturnOkAndCorrectContentType()
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

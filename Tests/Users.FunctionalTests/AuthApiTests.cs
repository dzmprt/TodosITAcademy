
using Auth.Application.Dtos;
using Auth.Application.Handlers.Commands.CreateJwtToken;
using Auth.Application.Handlers.Commands.CreateJwtTokenByRefreshToken;
using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Users.FunctionalTests
{
    public class AuthApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public AuthApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Login_Jwt_ReturnsToken()
        {
            // Arrange
            var loginCommand = new CreateJwtTokenCommand
            {
                Login = "admin",
                Password = "12345678"
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/LoginJwt"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(loginCommand), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseJson = await response.Content.ReadAsStringAsync();
                var tokenDto = JsonSerializer.Deserialize<JwtTokenDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(tokenDto.JwtToken);
            }
        }

        [Fact]
        public async Task Refresh_Jwt_ReturnsToken()
        {
            // Arrange
            var refreshTokenCommand = new CreateJwtTokenByRefreshTokenCommand
            {
                RefreshToken = "test_refresh_token"
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/RefreshJwt"))
            {
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(refreshTokenCommand), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseJson = await response.Content.ReadAsStringAsync();
                var tokenDto = JsonSerializer.Deserialize<JwtTokenDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(tokenDto.JwtToken);
            }
        }

    }
}

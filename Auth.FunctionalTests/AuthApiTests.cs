using Core.Tests.Attributes;
using System.Net.Http.Headers;
using System.Net;
using System.Text.Json;
using System.Text;
using Auth.Application.Handlers.Commands.CreateJwtToken;
using Microsoft.AspNetCore.Mvc.Testing;
using Auth.Application.Dtos;
using Castle.Core.Internal;

namespace Auth.FunctionalTests
{
    public class AuthApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        /*
        {
  "jwtToken": "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVGVzdENsaWVudDEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3NTkzMTEyLWNkOGQtNGM5Ni04OTNiLWY1M2M4Y2MzMWNkYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkNsaWVudCIsImV4cCI6MTc0MjY2MjU3NCwiaXNzIjoiVG9kb3MiLCJhdWQiOiJUb2RvcyJ9.Ybed_55LO5ktT8nVnWzMDoiy5fL9SFFcPdFcXcRJJ1Y",
  "refreshToken": "17593112-cd8d-4c96-893b-f53c8cc31cda",
  "expires": "2025-03-22T16:56:14.965718Z"
}
         */

        private const string adminToken =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiVGVzdENsaWVudDEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjE3NTkzMTEyLWNkOGQtNGM5Ni04OTNiLWY1M2M4Y2MzMWNkYSIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkNsaWVudCIsImV4cCI6MTc0MjY2MjU3NCwiaXNzIjoiVG9kb3MiLCJhdWQiOiJUb2RvcyJ9.Ybed_55LO5ktT8nVnWzMDoiy5fL9SFFcPdFcXcRJJ1Y";

        public AuthApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }


        [Theory]
        [FixtureInlineAutoData("TestClient1", "12345678")]
        public async Task Create_Jwt_ReturnCreatedAndCorrectContentType(string login, string password)
        {
            // Arrange
            var command = new CreateJwtTokenCommand()
            {
                Login = login,
                Password = password 
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/auth/api/v1/LoginJwt"))
            {
                requestMessage.Headers.Authorization =
                   new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(requestMessage);
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<JwtTokenDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.False(responseObject!.JwtToken.IsNullOrEmpty());
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
    }
}
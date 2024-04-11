using Core.Tests.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Todos.FunctionalTests
{
    public class TodosApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
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

        public TodosApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }
        #region query
        [Theory]
        [FixtureInlineAutoData(null, null)]
        [FixtureInlineAutoData(null, 1)]
        [FixtureInlineAutoData(1, null)]
        [FixtureInlineAutoData(1, 1)]
        public async Task Get_Todos_ReturnSuccessAndCorrectContentType(int? limit, int? offset)
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
                   new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/api/v1/Todos", query)))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(query), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                // Assert
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }
        [Fact]
        public async Task Get_Todo_ReturnSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();
            var query = new GetTodoQuery
            {
                TodoId = 1
            };

            // Act
            using (var requestMessage =
                   new HttpRequestMessage(HttpMethod.Get, $"/api/v1/Todos/{query.TodoId}"))
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
        [FixtureInlineAutoData("TestClient1 todo 1", 11)]
        [FixtureInlineAutoData(null , 30)]
        [FixtureInlineAutoData("", 30)]
        [FixtureInlineAutoData("TestClient3", 0)]
        public async Task Get_TodosCount_ReturnSuccessAndQuantityAndCorrectContentType(string todoName, int qt)
        {
            // Arrange
            var client = _factory.CreateClient();

            var query = new Dictionary<string, string?>
            {
                ["FreeText"] = todoName,
            };

            // Act
            using (var requestMessage =
                   new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("/api/v1/Todos/Count", query)))
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
        #endregion

        #region commands
        [Theory]
        [FixtureInlineAutoData("1")]
        [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890")]
        public async Task Create_Todo_ReturnCreatedAndCorrectContentType(string todoName)
        {
            // Arrange
            var command = new CreateTodoCommand()
            {
                Name = todoName
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Todos"))
            {
                requestMessage.Headers.Authorization =
                   new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(requestMessage);
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(responseObject!.Name, command.Name);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [FixtureInlineAutoData(1, "1", true)]
        [FixtureInlineAutoData(1, "12345678901234567890123456789012345678901234567890", false)]
        public async Task Update_Todo_ReturnUpdatedAndCorrectContentType(int todoId, string todoName, bool isDone)
        {
            // Arrange
            var command = new UpdateTodoCommand()
            {
                TodoId = todoId,
                Name = todoName,
                IsDone = isDone
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/Todos/{command.TodoId}"))
            {
                requestMessage.Headers.Authorization =
                   new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(requestMessage);
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(responseObject!.Name, command.Name);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Theory]
        [FixtureInlineAutoData(true)]
        [FixtureInlineAutoData(false)]
        public async Task SetIsDone_Todo_ReturnUpdatedAndCorrectContentType(bool isDone)
        {
            // Arrange
            var command = new UpdateTodoIsDoneCommand()
            {
                TodoId = 1,
                IsDone = isDone
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/Todos/{command.TodoId}/IsDone"))
            {
                requestMessage.Headers.Authorization =
                   new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
                var response = await client.SendAsync(requestMessage);
                var responseJson = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(responseObject!.IsDone, command.IsDone);
                Assert.Equal("application/json; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [Fact]
        public async Task Delete_Todo_ReturnOk()
        {
            // Arrange
            var command = new DeleteTodoCommand()
            {
                TodoId = 1
            };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/Todos/{command.TodoId}"))
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

        #endregion
    }
}
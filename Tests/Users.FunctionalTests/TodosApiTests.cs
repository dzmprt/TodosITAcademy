using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.DTOs;
using Core.Tests.Attributes;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;

namespace Users.FunctionalTests
{
    public class TodosApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        private const string adminToken =
            "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMjUyMTc4LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0.Cs2O0VLJFtq_jEsT4DPzhIkjRhfwi9SJsW5ojMJd4Ps";

        public TodosApiTests(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Put_Todo_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            var todoId = 2;
            var newTodoName = "NewTodoName";
            var isDone = true;
            var payload = new UpdateTodoPayload { Name = newTodoName, IsDone = isDone };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, $"/api/v1/Todos/{todoId}"))
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
                var updatedTodo = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(updatedTodo);
                Assert.Equal(todoId, updatedTodo.TodoId);
                Assert.Equal(newTodoName, updatedTodo.Name);
                Assert.Equal(isDone, updatedTodo.IsDone);
            }
        }

        [Fact]
        public async Task Get_TodosCount_ReturnsSuccessAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage =
                new HttpRequestMessage(HttpMethod.Get, "/api/v1/Todos/Count"))
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

        [Fact]
        public async Task GetTodo_ReturnsValidTodo()
        {
            // Arrange
            var todoId = 1;
            var client = _factory.CreateClient();

            // Act
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
            var response = await client.GetAsync($"/api/v1/Todos/{todoId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseJson = await response.Content.ReadAsStringAsync();
            var todo = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            Assert.NotNull(todo);
            Assert.Equal(todoId, todo.TodoId);
        }

        [Fact]
        public async Task Post_Todo_ReturnsCreatedAndCorrectContentType()
        {
            // Arrange
            var client = _factory.CreateClient();
            var command = new CreateTodoCommand { Name = "NewTodo" };

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "/api/v1/Todos"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content = new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal("application/json; charset=utf-8", response.Content.Headers.ContentType.ToString());

                var responseJson = await response.Content.ReadAsStringAsync();
                var createdTodo = JsonSerializer.Deserialize<GetTodoDto>(responseJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(createdTodo);
                Assert.Equal(command.Name, createdTodo.Name);
            }
        }

        [Fact]
        public async Task Get_Todos_ReturnsTodos()
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, "/api/v1/Todos"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", adminToken);

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseJson = await response.Content.ReadAsStringAsync();
                var todos = JsonSerializer.Deserialize<GetTodoDto[]>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(todos);
                Assert.NotEmpty(todos);
            }
        }

        [Fact]
        public async Task Patch_Todo_IsDone_ReturnsSuccess()
        {
            // Arrange
            var todoId = 2;
            var isDone = true;
            var payload = new UpdateTodoIsDonePayload { IsDone = isDone };
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Patch, $"/api/v1/Todos/{todoId}/IsDone"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", adminToken);
                requestMessage.Content =
                    new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);

                var responseJson = await response.Content.ReadAsStringAsync();
                var updatedTodo = JsonSerializer.Deserialize<GetTodoDto>(responseJson,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                Assert.NotNull(updatedTodo);
                Assert.Equal(todoId, updatedTodo.TodoId);
                Assert.Equal(isDone, updatedTodo.IsDone);
            }
        }

        [Fact]
        public async Task Delete_Todo_ReturnsSuccess()
        {
            // Arrange
            var todoId = 2;
            var client = _factory.CreateClient();

            // Act
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"/api/v1/Todos/{todoId}"))
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", adminToken);

                var response = await client.SendAsync(requestMessage);

                // Assert
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}





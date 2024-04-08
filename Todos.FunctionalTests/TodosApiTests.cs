using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Core.Tests.Attributes;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.WebUtilities;
using Todos.Applications.DTOs;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;

namespace Todos.FunctionalTests;

public class TodosApiTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private const string adminToken =
        "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiQWRtaW4iLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjM1ZjM3MzQwLWY5ZTUtNDExOC1iOTQ5LTA4ZGM1MWNjNTdiNyIsImh0dHA6Ly9zY2hlbWFzLm1pY3Jvc29mdC5jb20vd3MvMjAwOC8wNi9pZGVudGl0eS9jbGFpbXMvcm9sZSI6IkFkbWluIiwiZXhwIjoxNzQyMjUyMTc4LCJpc3MiOiJUb2RvcyIsImF1ZCI6IlRvZG9zIn0.Cs2O0VLJFtq_jEsT4DPzhIkjRhfwi9SJsW5ojMJd4Ps";

    public TodosApiTests(CustomWebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

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
               new HttpRequestMessage(HttpMethod.Get, QueryHelpers.AddQueryString("api/v1/Todos", query)))
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
    public async Task Create_Todo_ReturnCreatedAndCorrectContentType()
    {
        // Arrange
        var command = new CreateTodoCommand()
        {
            Name = "test",
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, "api/v1/Todos"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

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

/*    [Fact]
    public async Task Update_Todo_ReturnOKAndCorrectContentType()
    {
        // Arrange
        var command = new UpdateTodoCommand()
        {
            TodoId = 1,
            Name = "test",
            IsDone = true,
        };
        var client = _factory.CreateClient();

        // Act
        using (var requestMessage = new HttpRequestMessage(HttpMethod.Put, "api/v1/Todos"))
        {
            requestMessage.Content =
                new StringContent(JsonSerializer.Serialize(command), Encoding.UTF8, "application/json");
            requestMessage.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", adminToken);

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
    }*/

}

using AutoFixture;
using AutoFixture.Xunit2;
using Core.Tests;
using FluentValidation;
using Todos.Applications.Handlers.Queries.GetTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo;

public class GetTodoQueryValidatorTest : ValidatorTestBase<GetTodoQuery>
{
    public GetTodoQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IValidator<GetTodoQuery> TestValidator =>
        TestFixture.Create<GetTodoQueryValidator>();

    [Theory]
    [InlineAutoData(1)]
    [InlineAutoData(10)]
    [InlineAutoData(20)]
    public void Should_BeValid_When_RequestIsValid(int todoId)
    {
        // arrange
        var query = new GetTodoQuery
        {
            TodoId = todoId
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [InlineAutoData(-1)]
    [InlineAutoData(0)]
    public void Should_BeNotValid_When_RequestIsInvalid(int todoId)
    {
        // arrange
        var query = new GetTodoQuery
        {
            TodoId = todoId
        };

        // act & assert
        AssertNotValid(query);
    }
}
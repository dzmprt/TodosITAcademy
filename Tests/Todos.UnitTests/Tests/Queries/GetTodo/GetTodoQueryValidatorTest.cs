using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
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
    [FixtureInlineAutoData(42)]
    [FixtureInlineAutoData(5)]
    [FixtureInlineAutoData(1)]
    public void Should_BeValid_When_IdIsValid(int id)
    {

        var command = new GetTodoQuery
        {
            TodoId = id,
        };

        AssertValid(command);
    }

    [Theory]
    [FixtureInlineAutoData(0)]
    [FixtureInlineAutoData(-5)]
    public void Should_NotValid_When_IdZeroAndLower(int id)
    {
        var command = new GetTodoQuery
        {
            TodoId = id,
        };

        AssertNotValid(command);
    }
}

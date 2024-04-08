using AutoFixture;
using AutoFixture.Xunit2;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo;

public class DeleteTodoCommandValidatorTest : ValidatorTestBase<DeleteTodoCommand>
{
    public DeleteTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IValidator<DeleteTodoCommand> TestValidator =>
        TestFixture.Create<DeleteTodoCommandValidator>();

    [Theory]
    [InlineAutoData(1)]
    [InlineAutoData(5)]
    [InlineAutoData(10)]
    public void Should_BeValid_When_RequestIsValid(int todoId)
    {
        // arrange
        var query = new DeleteTodoCommand
        {
            TodoId = todoId,
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [FixtureInlineAutoData(-1)]
    [FixtureInlineAutoData(0)]
    public void Should_NotValid_When_RequestIsInvalid(int todoId, string name)
    {
        // arrange
        var query = new DeleteTodoCommand
        {
            TodoId = todoId,
        };

        // act & assert
        AssertNotValid(query);
    }
}
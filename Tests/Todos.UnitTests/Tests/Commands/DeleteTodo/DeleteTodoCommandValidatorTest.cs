using AutoFixture;
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
    [FixtureInlineAutoData(42)]
    [FixtureInlineAutoData(5)]
    [FixtureInlineAutoData(1)]
    public void Should_BeValid_When_IdIsValid(int id)
    {

        var command = new DeleteTodoCommand
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
        var command = new DeleteTodoCommand
        {
            TodoId = id,
        };

        AssertNotValid(command);
    }
}

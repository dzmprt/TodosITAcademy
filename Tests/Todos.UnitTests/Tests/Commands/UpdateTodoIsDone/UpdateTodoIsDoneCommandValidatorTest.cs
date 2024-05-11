using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone;

public class UpdateTodoIsDoneCommandValidatorTest : ValidatorTestBase<UpdateTodoIsDoneCommand>
{
    public UpdateTodoIsDoneCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IValidator<UpdateTodoIsDoneCommand> TestValidator =>
        TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

    [Theory]
    [FixtureInlineAutoData(42)]
    [FixtureInlineAutoData(5)]
    [FixtureInlineAutoData(1)]
    public void Should_BeValid_When_IdIsValid(int id)
    {

        var command = new UpdateTodoIsDoneCommand
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
        var command = new UpdateTodoIsDoneCommand
        {
            TodoId = id,
        };

        AssertNotValid(command);
    }
}

using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo;

public class UpdateTodoCommandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
{
    public UpdateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IValidator<UpdateTodoCommand> TestValidator =>
        TestFixture.Create<UpdateTodoCommandValidator>();

    [Theory]
    [FixtureInlineAutoData("123")]
    [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
    [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890")]
    public void Should_BeValid_When_NameIsValid(string name)
    {

        var command = new UpdateTodoCommand
        {
            Name = name,
            TodoId = 1,
            IsDone = true
        };

        AssertValid(command);
    }

    [Theory]
    [FixtureInlineAutoData(null)]
    [FixtureInlineAutoData("")]
    public void Should_NotValid_When_NameEmpty(string name)
    {
        var command = new UpdateTodoCommand
        {
            Name = name,
            TodoId = 1,
            IsDone = true
        };

        AssertNotValid(command);
    }

    [Fact]
    public void Should_NotBeValid_When_NameIsGreatThen200()
    {
        var query = new UpdateTodoCommand
        {
            Name = "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901",
            TodoId = 1,
            IsDone = true
        };

        AssertNotValid(query);
    }

    [Theory]
    [FixtureInlineAutoData(42)]
    [FixtureInlineAutoData(5)]
    [FixtureInlineAutoData(1)]
    public void Should_BeValid_When_IdIsValid(int id)
    {

        var command = new UpdateTodoCommand
        {
            Name = "123",
            TodoId = id,
            IsDone = true
        };

        AssertValid(command);
    }

    [Theory]
    [FixtureInlineAutoData(0)]
    [FixtureInlineAutoData(-5)]
    public void Should_NotValid_When_IdZeroAndLower(int id)
    {
        var command = new UpdateTodoCommand
        {
            Name = "123",
            TodoId = id,
            IsDone = true
        };

        AssertNotValid(command);
    }
}

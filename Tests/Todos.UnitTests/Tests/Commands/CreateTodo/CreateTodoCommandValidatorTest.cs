using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo;

public class CreateTodoCommandValidatorTest : ValidatorTestBase<CreateTodoCommand>
{
    public CreateTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }

    protected override IValidator<CreateTodoCommand> TestValidator =>
        TestFixture.Create<CreateTodoCommandValidator>();

    [Theory]
    [FixtureInlineAutoData("123")]
    [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
    [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890")]
    public void Should_BeValid_When_NameIsValid(string name)
    {

        var command = new CreateTodoCommand
        {
            Name = name,
        };

        AssertValid(command);
    }
    [Theory]
    [FixtureInlineAutoData(null)]
    [FixtureInlineAutoData("")]
    public void Should_NotValid_When_NameEmpty(string name)
    {
        var command = new CreateTodoCommand
        {
            Name = name,
        };

        AssertNotValid(command);
    }

    [Fact]
    public void Should_NotBeValid_When_NameIsGreatThen200()
    {
        var query = new CreateTodoCommand
        {
            Name = "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901" +
            "123456789012345678901234567890123456789012345678901",
        };

        AssertNotValid(query);
    }
}

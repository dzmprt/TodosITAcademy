using AutoFixture;
using AutoFixture.Xunit2;
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
    [InlineAutoData("1")]
    [InlineAutoData("123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-")]
    public void Should_BeValid_When_RequestIsValid(string name)
    {
        // arrange
        var query = new CreateTodoCommand
        {
            Name = name,
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [FixtureInlineAutoData("")]
    [FixtureInlineAutoData("123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-1")]
    public void Should_NotValid_When_RequestIsInvalid(string name)
    {
        // arrange
        var query = new CreateTodoCommand
        {
            Name = name,
        };

        // act & assert
        AssertNotValid(query);
    }
}
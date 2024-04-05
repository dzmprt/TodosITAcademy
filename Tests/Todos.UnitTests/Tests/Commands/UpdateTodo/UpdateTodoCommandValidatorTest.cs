using AutoFixture;
using AutoFixture.Xunit2;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.DTOs;
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
    [InlineAutoData(1, "123456789-")]
    [InlineAutoData(5, "123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-")]
    public void Should_BeValid_When_RequestIsValid(int todoId, string name)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = todoId,
            Name = name,
            IsDone = true,
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [FixtureInlineAutoData(-1, "")]
    [FixtureInlineAutoData(1, "")]
    [FixtureInlineAutoData(1, "123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-1")]
    public void Should_NotValid_When_RequestIsInvalid(int todoId, string name)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = todoId,
            Name = name,
            IsDone = true,
        };

        // act & assert
        AssertNotValid(query);
    }
}
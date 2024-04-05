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
    [InlineAutoData(1)]
    [InlineAutoData(5)]
    [InlineAutoData(10)]
    public void Should_BeValid_When_TodoIdIsValid(int todoId)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = todoId,
            Name = "123",
            IsDone = true,
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [InlineAutoData("123456789-")]
    [InlineAutoData("123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-")]
    public void Should_BeValid_When_NameIsValid(string name)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = 1,
            Name = name,
            IsDone = true,
        };

        // act & assert
        AssertValid(query);
    }

    [Theory]
    [FixtureInlineAutoData(0)]
    [FixtureInlineAutoData(-1)]
    public void Should_NotValid_When_TodoIdIsInvalid(int todoId)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = todoId,
            Name = "123",
            IsDone = true,
        };

        // act & assert
        AssertNotValid(query);
    }

    [Theory]
    [FixtureInlineAutoData("")]
    [FixtureInlineAutoData("123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-123456789-1")]
    public void Should_NotValid_When_NameIsInvalid(string name)
    {
        // arrange
        var query = new UpdateTodoCommand
        {
            TodoId = 1,
            Name = name,
            IsDone = true,
        };

        // act & assert
        AssertNotValid(query);
    }

}
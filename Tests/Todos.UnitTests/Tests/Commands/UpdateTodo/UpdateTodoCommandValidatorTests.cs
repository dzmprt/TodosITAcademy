using Core.Tests;
using FluentValidation;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;
using AutoFixture;
using Core.Tests.Attributes;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoCommandValidatorTests: ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoCommandValidatorTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        protected override IValidator<UpdateTodoCommand> TestValidator =>
            TestFixture.Create<UpdateTodoCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(200)]
        public void Should_BeValid_When_NameIsValid(int countLetters)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                TodoId = 1,
                Name = new string('T',countLetters),
            };

            // act & assert
            AssertValid(query);
        } 
        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(int.MaxValue)]
        public void Should_BeValid_When_TodoIdIsValid(int id)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                Name = new string('T', 200),
                TodoId = id
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(201)]
        public void Should_NotValid_When_NameIsNotValid(int countLetters)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                Name = countLetters == 0 ? string.Empty :new string('t', countLetters),
                
            };

            // act & assert
            AssertNotValid(query);
        }
        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotValid_When_TodoIdIsNotValid(int id)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                Name = new string('T', 200),
                TodoId =id
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

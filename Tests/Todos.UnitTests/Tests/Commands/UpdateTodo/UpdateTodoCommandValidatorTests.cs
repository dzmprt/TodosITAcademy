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
        [FixtureInlineAutoData(1, 1)]
        [FixtureInlineAutoData(1, 200)]
        [FixtureInlineAutoData(int.MaxValue, 1)]
        [FixtureInlineAutoData(int.MaxValue, 200)]
        public void Should_BeValid_When_RequestIsValid(int id, int countLetters)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                Name = new string('t',countLetters),
                TodoId = id
                
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0,0)]
        [FixtureInlineAutoData(0,201)]
        [FixtureInlineAutoData(-1,0)]
        [FixtureInlineAutoData(-1,201)]
        public void Should_NotValid_When_RequesIsNotValid(int id, int countLetters)
        {
            // arrange
            var query = new UpdateTodoCommand()
            {
                Name = countLetters == 0 ? string.Empty :new string('t', countLetters),
                TodoId=id
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

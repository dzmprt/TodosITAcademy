using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Queries.GetTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodo
{
    public class GetTodoQueryValidatorTest : ValidatorTestBase<GetTodoQuery>
    {
        public GetTodoQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        { 
        }

        protected override IValidator<GetTodoQuery> TestValidator =>
            TestFixture.Create<GetTodoQueryValidator>();

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(15)]
        [FixtureInlineAutoData(1555)]
        public void Should_BeValid_When_TodoIdIsValid(int todoId)
        {
            // arrange
            var query = new GetTodoQuery
            {
                TodoId = todoId,
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_IncorrectTodoId(int todoId)
        {
            // arrange
            var query = new GetTodoQuery
            {
                TodoId = todoId,
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

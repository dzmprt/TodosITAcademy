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

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new GetTodoQuery
            {
                TodoId = 1
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(1)]
        [FixtureInlineAutoData(10)]
        [FixtureInlineAutoData(20)]
        public void Should_Valid_When_TodoIdIsValid(int id)
        {
            // arrange
            var query = new GetTodoQuery
            {
                TodoId = id
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        [FixtureInlineAutoData(-10)]
        public void Should_NotBeValid_When_TodoIdIncorrect(int id)
        {
            // arrange
            var query = new GetTodoQuery
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

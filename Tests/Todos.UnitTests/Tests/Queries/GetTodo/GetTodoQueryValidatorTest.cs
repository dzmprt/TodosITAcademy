using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Queries.GetTodo;
using Xunit.Abstractions;
using AutoFixture;

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
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_TodoIdIsNotPositive(int todoId)
        {
            // Arrange
            var query = new GetTodoQuery
            {
                TodoId = todoId
            };

            // Act & Assert
            AssertNotValid(query);
        }

        [Fact]
        public void Should_BeValid_When_TodoIdIsValid()
        {
            // Arrange
            var query = new GetTodoQuery
            {
                TodoId = 1
            };

            // Act & Assert
            AssertValid(query);
        }
    }

}

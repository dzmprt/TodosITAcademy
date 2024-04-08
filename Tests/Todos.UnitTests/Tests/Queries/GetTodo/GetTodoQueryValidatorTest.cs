using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Applications.Handlers.Queries.GetTodos;
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
                TodoId = 1,
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(null)]
        [FixtureInlineAutoData(-5)]
        [FixtureInlineAutoData(0)]
        public void Should_NotBeValid_When_IncorrectGuid(int id)
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

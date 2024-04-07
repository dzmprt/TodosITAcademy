using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Todos.Applications.Handlers.Queries.GetTodo;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount
{
    public class GetTodosCountQueryValidatorTest : ValidatorTestBase<GetTodosCountQuery>
    {
        public GetTodosCountQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<GetTodosCountQuery> TestValidator =>
            TestFixture.Create<GetTodosCountQueryValidator>();
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("1")]
        [InlineData("12345678901234567890123456789012345678901234567890")]
        public void Should_BeValid_When_RequestValid(string? name)
        {
            // arrange
            var query = new GetTodosCountQuery
            {
                FreeText = name,
            };

            // act & assert
            AssertValid(query);
        }

        [Fact]
        public void Should_BeNotValid_When_RequestNotValid()
        {
            // arrange
            var query = new GetTodosCountQuery
            {
                FreeText = "123456789012345678901234567890123456789012345678901",
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

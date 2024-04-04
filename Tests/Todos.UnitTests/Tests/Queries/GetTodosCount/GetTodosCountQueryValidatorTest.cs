using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Todos.Applications.Handlers.Queries;
using Xunit.Abstractions;
using AutoFixture;
using Todos.Domain;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount
{
    public class GetTodosCountQueryValidatorTest : ValidatorTestBase<GetTodosCountQuery>
    {
        public GetTodosCountQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<GetTodosCountQuery> TestValidator =>
            TestFixture.Create<GetTodosCountQueryValidator>();
    
        [Fact]
        public void Should_BeValid_When_FreeTextIsValid()
        {
            // Arrange
            var query = new GetTodosCountQuery
            {
                FreeText = "Valid FreeText"
            };

            // Act & Assert
            AssertValid(query);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void Should_BeValid_When_FreeTextIsNullOrEmpty(string freeText)
        {
            // Arrange
            var query = new GetTodosCountQuery
            {
                FreeText = freeText
            };

            // Act & Assert
            AssertValid(query);
        }

        [Theory]
        [InlineData("123456789012345678901234567890123456789012345678901")]
        public void Should_NotBeValid_When_FreeTextExceedsMaximumLength(string freeText)
        {
            // Arrange
            var query = new GetTodosCountQuery
            {
                FreeText = freeText
            };

            // Act & Assert
            AssertNotValid(query);
        }
    }

}

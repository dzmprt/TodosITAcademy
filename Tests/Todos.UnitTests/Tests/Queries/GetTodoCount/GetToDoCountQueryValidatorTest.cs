using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodoCount
{
    public class GetToDoCountQueryValidatorTest : ValidatorTestBase<GetTodosCountQuery>
    {

        public GetToDoCountQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<GetTodosCountQuery> TestValidator =>
            TestFixture.Create<GetTodosCountQueryValidator>();


        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new GetTodosCountQuery
            {
                FreeText = "123",
                IsDone = true
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData("")]
        [FixtureInlineAutoData(null)]
        [FixtureInlineAutoData("123")]
        [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
        [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890")]
        public void Should_BeValid_When_FreeTextIsValid(string freeText)
        {
            // arrange
            var query = new GetTodosCountQuery
            {
                FreeText = freeText,
            };

            // act & assert
            AssertValid(query);
        }

        [Fact]
        public void Should_NotBeValid_When_FreeTextIsGreatThen50()
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

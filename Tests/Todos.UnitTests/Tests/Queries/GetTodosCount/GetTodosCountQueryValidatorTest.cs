using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Queries.GetTodos;
using Todos.Applications.Handlers.Queries.GetTodosCount;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount;

public class GetTodosCountQueryValidatorTest: ValidatorTestBase<GetTodosCountQuery>
{
    public GetTodosCountQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
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
            IsDone = true,
        };

        // act & assert
        AssertValid(query);
    }
    [Fact]
    public void Should_BeValid_WithEmptyParameters()
    {
        // arrange
        var query = new GetTodosCountQuery();

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
            FreeText = new string('a', 51),
        };

        // act & assert
        AssertNotValid(query);
    }
}
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using Todos.Applications.Handlers.Queries.GetTodos;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Queries.GetTodosCount;

public class GetTodosCountQueryValidatorTest : ValidatorTestBase<GetTodosCount>
{
    public GetTodosQueryValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
    {
    }
    protected override IValidator<GetTodosQuery> TestValidator =>
    TestFixture.Create<GetTodosQueryValidator>();

    [Theory]
    [FixtureInlineAutoData("")]
    [FixtureInlineAutoData(null)]
    [FixtureInlineAutoData("123")]
    [FixtureInlineAutoData("1#*&^%$#@#$%±~`}{][\\|?/.,<>")]
    [FixtureInlineAutoData("12345678901234567890123456789012345678901234567890")]
    public void Should_BeValid_When_FreeTextIsValid(string freeText)
    {
        // arrange
        var query = new GetTodosQuery
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
        var query = new GetTodosQuery
        {
            FreeText = "123456789012345678901234567890123456789012345678901",
        };

        // act & assert
        AssertNotValid(query);
    }
}

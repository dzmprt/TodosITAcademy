using AutoFixture;
using Core.Tests;
using Core.Tests.Attributes;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Queries.GetTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodo
{
    public class UpdateTodoComandValidatorTest : ValidatorTestBase<UpdateTodoCommand>
    {
        public UpdateTodoComandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoCommand> TestValidator =>
        TestFixture.Create<UpdateTodoCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new UpdateTodoCommand
            {
                TodoId = 1,
                Name = "qwerty",
                IsDone = false,
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(-5)]
        [FixtureInlineAutoData(0)]
        public void Should_NotBeValid_When_IncorrectGuid(int id)
        {
            // arrange
            var query = new UpdateTodoCommand
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

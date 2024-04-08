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
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Todos.Applications.Handlers.Queries.GetTodos;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneValidatorTest : ValidatorTestBase<UpdateTodoIsDoneCommand>
    {
        public UpdateTodoIsDoneValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator =>
        TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new UpdateTodoIsDoneCommand
            {
                TodoId = 1,
            };

            // act & assert
            AssertValid(query);
        }

        [Theory]
        [FixtureInlineAutoData(-5)]
        [FixtureInlineAutoData(0)]
        public void Should_NotBeValid_When_Id_NotValid(int id)
        {
            // arrange
            var query = new UpdateTodoIsDoneCommand
            {
                TodoId = id,
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

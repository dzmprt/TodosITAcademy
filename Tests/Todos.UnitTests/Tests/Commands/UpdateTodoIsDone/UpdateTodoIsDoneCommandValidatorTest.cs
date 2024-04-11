using AutoFixture;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandValidatorTest : ValidatorTestBase<UpdateTodoIsDoneCommand>
    {
        public UpdateTodoIsDoneCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator => TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var update = new UpdateTodoIsDoneCommand
            {
                IsDone = true,
                TodoId = 1,
            };

            // act & assert
            AssertValid(update);
        }
        
        [Fact]
        public void Should_NotValid_With_ZeroId()
        {
            // arrange
            var update = new UpdateTodoIsDoneCommand
            {
                IsDone = true,
                TodoId = 0
            };

            // act & assert
            AssertNotValid(update);
        }

    }
}

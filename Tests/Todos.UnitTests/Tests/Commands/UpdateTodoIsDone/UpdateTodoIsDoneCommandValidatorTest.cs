using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.UpdateTodoIsDone;
using Xunit.Abstractions;
using AutoFixture;

namespace Todos.UnitTests.Tests.Commands.UpdateTodoIsDone
{
    public class UpdateTodoIsDoneCommandValidatorTest : ValidatorTestBase<UpdateTodoIsDoneCommand>
    {
        public UpdateTodoIsDoneCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<UpdateTodoIsDoneCommand> TestValidator =>
            TestFixture.Create<UpdateTodoIsDoneCommandValidator>();

        [Theory]
        [FixtureInlineAutoData(0)]
        [FixtureInlineAutoData(-1)]
        public void Should_NotBeValid_When_TodoIdIsNotPositive(int todoId)
        {
            // Arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = todoId
            };

            // Act & Assert
            AssertNotValid(command);
        }

        [Fact]
        public void Should_BeValid_When_TodoIdIsValid()
        {
            // Arrange
            var command = new UpdateTodoIsDoneCommand
            {
                TodoId = 1
            };

            // Act & Assert
            AssertValid(command);
        }
    }

}


using Core.Tests.Attributes;
using Core.Tests;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.UpdateTodo;
using Xunit.Abstractions;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using AutoFixture;
using Todos.Applications.Handlers.Queries.GetTodo;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
    public class DeleteTodoComandValidator : ValidatorTestBase<DeleteTodoCommand>
    {
        public DeleteTodoComandValidator(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }
        protected override IValidator<DeleteTodoCommand> TestValidator =>
            TestFixture.Create<DeleteTodoCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new DeleteTodoCommand
            {
                TodoId = 1,
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
            var query = new DeleteTodoCommand
            {
                TodoId = id
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

using AutoFixture;
using Core.Tests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Todos.Applications.Handlers.Commands.CreateTodo;
using Todos.Applications.Handlers.Commands.DeleteTodo;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.DeleteTodo
{
    public class DeleteTodoCommandValidatorTest : ValidatorTestBase<DeleteTodoCommand>
    {
        public DeleteTodoCommandValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override FluentValidation.IValidator<DeleteTodoCommand> TestValidator => TestFixture.Create<DeleteTodoCommandValidator>();

        [Theory]
        [InlineData(1)]
        [InlineData(int.MaxValue)]
        public void Should_BeValid_When_Id_Valid(int id)
        {
            // arrange
            var create = new DeleteTodoCommand
            {
                TodoId = id
            };
            // act & assert
            AssertValid(create);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Should_NotValid_When_Id_NotValid(int id)
        {
            // arrange
            var create = new DeleteTodoCommand
            {
                TodoId = 0
            };
            // act & assert
            AssertNotValid(create);
        }
    }
}

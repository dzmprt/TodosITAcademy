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
using Todos.Applications.Handlers.Queries.GetTodos;
using Xunit;
using Xunit.Abstractions;

namespace Todos.UnitTests.Tests.Commands.CreateTodo
{
    public class CreateTodoValidatorTest : ValidatorTestBase<CreateTodoCommand>
    {
        public CreateTodoValidatorTest(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }

        protected override IValidator<CreateTodoCommand> TestValidator =>
        TestFixture.Create<CreateTodoCommandValidator>();

        [Fact]
        public void Should_BeValid_When_RequestIsValid()
        {
            // arrange
            var query = new CreateTodoCommand
            {
                Name = "name",
            };

            // act & assert
            AssertValid(query);
        }


        [Fact]
        public void Should_NotBeValid_When_Name_Greater_Than_200()
        {
            // arrange
            var query = new CreateTodoCommand
            {
                Name = "012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789012345678901234567890",
            };

            // act & assert
            AssertNotValid(query);
        }
    }
}

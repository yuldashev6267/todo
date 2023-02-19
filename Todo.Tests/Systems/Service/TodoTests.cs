using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Todo.Core;
using Todo.Database;
using Todo.Database.Entity;
using Todo.Service.Models;
using Todo.Service.Services;
using Todo.Tests.Mock;



namespace Todo.Tests
{
    public class TodoTest : IDisposable
    {
        protected readonly DatabaseContext _dbContext;

        protected readonly Mock<ITags> _tag;

        public TodoTest()
        {
            var options = new DbContextOptionsBuilder<DatabaseContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _dbContext = new DatabaseContext(options);

            _dbContext.Database.EnsureCreated();

            _tag = new Mock<ITags>();
        }

        [Fact]
        public async Task GetAll_OnSuccess_ReturnsAllTodoCount()
        {
            /// Arrange

            // Seed todos
            _dbContext.Todos.AddRange(TodoMockData.GetTodos().Todos);
            await _dbContext.SaveChangesAsync();

            var sut = new TodoService(_dbContext, _tag.Object);

            /// Act
            var result = await sut.GetAll(new GetAllModel());

            /// Assert
            result.Todos.Should().HaveCount(TodoMockData.GetTodos().Todos.Count);
        }

        [Fact]
        public async Task CreateTodo_OnSuccess_ReturnsResult()
        {
            /// Arrange

            var sut = new TodoService(_dbContext, _tag.Object);
            /// Act
            string[] tags = {};

            var todo = new AddTodoModel()
            {
                Title = "New Title",
                Description = "New Decription",
                Priority = Priority.High,
                Colour = Colour.Blue,
                Tags = tags
            };

            var result = await sut.AddTodo(todo);

            /// Assert
            result.Should().BeOfType<AddTodoResult>();
            result.Todo.Should().BeOfType<TodoEntity>();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }
}
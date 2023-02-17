using Moq;
using Todo.Service.Services;
using Todo.Tests.Mock;
using static Todo.Tests.Mock.TodoMockData;

namespace Todo.Tests
{
    public class TodoTest
    {
        [Fact]
        public async Task Test_1()
        {
            var todoService = new Mock<ITodo>();
            var configureSetup = todoService.Setup(_=>_.GetAll(null));
        }
    }
}


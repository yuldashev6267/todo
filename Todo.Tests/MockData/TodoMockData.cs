using Todo.Core;
using Todo.Database.Entity;
using Todo.Service.Models;

namespace Todo.Tests.Mock
{
    public class TodoMockData
    {
        public static GetAllResult GetTodos()
        {
            var result = new GetAllResult();
            List<TodoEntity> todos = new List<TodoEntity>()
            {   
                new TodoEntity()
                {
                    Id = 1,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsCompleted = false,
                    IsDeleted = false,
                    DeletedAt = null,
                    Title = "",
                    Description = "",
                    Colour = Colour.Blue,
                    Priority = Priority.High,
                    Tags = new List<TagEntity>()
                    {
                        new TagEntity()
                        {
                            Id = 1,
                            Tag = "this is test tag",
                            Usage = 1
                        }
                    }
                },
                new TodoEntity()
                {
                    Id = 2,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsCompleted = false,
                    IsDeleted = false,
                    DeletedAt = null,
                    Title = "",
                    Description = "",
                    Colour = Colour.Blue,
                    Priority = Priority.High
                }
            };

            result.Todos = todos;
            result.Success = true;

            return result;
        }
    }    
}


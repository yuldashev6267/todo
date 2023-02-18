using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using MySqlX.XDevAPI.Common;
using Todo.Core;
using Todo.Database.Entity;

namespace Todo.Service.Models
{
    public class AddTodoModel
    {
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public Priority? Priority { get; set; }
        public Colour? Colour { get; set; }

        public string[] Tags { get; set; }
    }

    public class AddTodoResult : BaseResultModel
    {
        public TodoEntity Todo { get; set; }
    }

    public class DeleteTodoResult : BaseResultModel
    {
        public TodoEntity Todo { get; set; }

        public bool NotFound { get; set; }
    }

    public class GetTodoResult : BaseResultModel
    {
        public TodoEntity Todo { get; set; }

        public bool NotFound { get; set; }
    }

    public class GetAllModel
    {
        public bool Desc { get; set; }
        public string? Tag { get; set; }
        public Priority? Priority { get; set; }
        public Colour? Colour { get; set; }
        public int? Skip { get; set; }
        public int? Limit { get; set; }
    }

    public class GetAllResult : BaseResultModel
    {
        public List<TodoEntity> Todos { get; set; }
    }

    public class RemoveTagFromTodoModel
    {
        public long TodoID { get; set; }

        public long TagID { get; set; }
    }

    public class RemoveTagFromTodoResult : BaseResultModel
    {
        public TodoEntity Todo { get; set; }

        public bool NotFound { get; set; }
    }

    public class SearchTodoResult : BaseResultModel
    {
        public IEnumerable<TodoEntity> Todos { get; set; }
    }

    public class EditTodoModel
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public Priority? Priority { get; set; }

        public Colour? Colour { get; set; }

        public string[] Tags { get; set; }
    }

    public class EditTodoResult : BaseResultModel
    {
        public bool NotFound { get; set; }

        public TodoEntity Todo { get; set; }
    }

    public class CompletedTodoResult : BaseResultModel
    {
        public TodoEntity Todo { get; set; }

        public bool NotFound { get; set; }
    }

    public class GetAllCountResult : BaseResultModel
    {
        public int Count { get; set; }
    }

    public class GetAllCountModel
    {
        public bool Desc { get; set; }
        public string? Tag { get; set; }
        public Priority? Priority { get; set; }
        public Colour? Colour { get; set; }
    }
}
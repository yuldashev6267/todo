using System.ComponentModel.DataAnnotations;
using Todo.Database.Entity;

namespace Todo.Service.Models
{

    public class AddTagResult : BaseResultModel
    {
        public TagEntity Tag { get; set; }
    }
    public class AddTagModel
    {
        [Required(AllowEmptyStrings = false)] public string Tag { get; set; }
    }

    public class GetTagByIdResult : BaseResultModel
    {
        public bool NotFound { get; set; }
        
        public TagEntity Tag { get; set; }
    }

    public class EditTodoResult : BaseResultModel
    {
        public bool NotFound { get; set; }
        
        public TodoEntity Todo { get; set; }
    }
}


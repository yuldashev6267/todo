using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Todo.Core;

namespace Todo.WebAPi.RequestModels
{
    public class CreateTodoRequestModel
    {
        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [EnumDataType(typeof(Colour), ErrorMessage = "You are not allowed to add this colour")]
        public Colour? Colour { get; set; }
        
        [EnumDataType(typeof(Priority), ErrorMessage = "Priority type not found")]
        public Priority? Priority { get; set; }
        
        public string[] Tags { get; set; }
    }

    public class GetAllRequestModel
    {
        public bool Desc { get; set; }
        
        public string? Tag { get; set; }
        
        public Priority? Priority { get; set; }
        
        public Colour? Colour { get; set; }
        
        public int? Skip { get; set; }
        
        public int? Limit { get; set; }
    }


    public class EditTodoRequestModel
    {
        [Required]
        [JsonPropertyName("id")] 
        public long Id { get; set; }
        
        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [Required]
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("color")]
        public Colour? Colour { get; set; }
        
        [JsonPropertyName("priority")]
        public Priority? Priority { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }

    }
}


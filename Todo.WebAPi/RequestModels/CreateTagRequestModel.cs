using System.ComponentModel.DataAnnotations;

namespace Todo.WebAPi.RequestModels
{
    public class CreateTagRequestModel
    {
        [Required]
        public string Tag { get; set; }
    } 
}


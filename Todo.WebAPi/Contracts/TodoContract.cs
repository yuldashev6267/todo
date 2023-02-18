using System.Text.Json.Serialization;
using Org.BouncyCastle.Crypto.Engines;
using Todo.Core;
using Todo.Database.Entity;
using Todo.WebAPi.Helpers;

namespace Todo.WebAPi.Contracts
{
    public class TodoContract
    {
        [JsonPropertyName("id")] public long Id { get; set; }
        
        [JsonPropertyName("created_at")] public string CreatedAt { get; set; }
        
        [JsonPropertyName("updated_at")] public string UpdatedAt { get; set; }
        
        [JsonPropertyName("is_deleted")] public bool IsDeleted { get; set; }
        
        [JsonPropertyName("deleted_at")] public string DeletedAt { get; set; }
        
        [JsonPropertyName("is_completed")] public bool IsCompleted { get; set; }
        
        [JsonPropertyName("title")] public string Title { get; set; }
        
        [JsonPropertyName("description")] public string Description { get; set; }
        
        [JsonPropertyName("priority_level")] public string PriorityLevel { get; set; }
        
        [JsonPropertyName("color")] public  string Color { get; set; }
        
        [JsonPropertyName("tags")]
        public List<TagContract> Tags { get; set; }

        public static TodoContract ConvertEntityToContract(TodoEntity todo)
        {
            var contract = new TodoContract();
            List<TagContract> tags = new List<TagContract>();
            contract.Id = todo.Id;
            contract.CreatedAt = todo.CreatedAt.ConvertToDateTime();
            contract.UpdatedAt = todo.UpdatedAt.ConvertToDateTime();
            contract.IsDeleted = todo.IsDeleted;
            contract.DeletedAt = todo.DeletedAt.HasValue ? todo.DeletedAt.Value.ConvertToDateTime() : null;
            contract.IsCompleted = todo.IsCompleted;
            contract.Title = todo.Title;
            contract.Description = todo.Description;
            contract.PriorityLevel = ConvertPriorityToString(todo.Priority);
            contract.Color = ConvertColourToString(todo.Colour);
            contract.Tags = todo.Tags?.Select(TagContract.TagContractFromEntity).ToList();
            
            
            return contract;
        }

        private static string ConvertPriorityToString(Priority priority)
        {
            switch (priority)
            {
                case  Priority.Low:
                    return "Low";
                case Priority.Medium:
                    return "Medium";
                case Priority.High:
                    return "High";
                default:
                    return "Priority level not set";
            }
        }

        private static string ConvertColourToString(Colour colour)
        {
            switch (colour)
            {
                case Colour.Blue:
                    return "Blue";
                case Colour.Green:
                    return "Green";
                case Colour.Grey:
                    return "Grey";
                case Colour.Orange:
                    return "Orange";
                case Colour.Purple:
                    return "Purple";
                case Colour.Red:
                    return "Red";
                case Colour.Yellow:
                    return "Yellow";
                default:
                    return "White";
            }
        }
    }    
}


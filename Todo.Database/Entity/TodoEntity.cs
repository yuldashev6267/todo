using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Todo.Core;

namespace Todo.Database.Entity
{
    [Table("Todos")]
    public class TodoEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt { get; set; }
        
        [Column("updated_At")]
        public DateTime UpdatedAt { get; set; }
        
        [Column("deleted_At")]
        public DateTime? DeletedAt { get; set; }
        
        [Column("is_deleted")]
        public bool IsDeleted { get; set; }
        
        [Column("is_complted")]
        public bool IsCompleted { get; set; }

        [Column("priority")] public Priority Priority { get; set; } = Core.Priority.None;
        
        [Column("title")]
        public string Title { get; set; }
        
        [Column("description")]
        public string Description { get; set; }

        [Column("colour")] public Colour Colour { get; set; } = Core.Colour.Blue;
        
        public ICollection<TagEntity> Tags { get; set; }
    }    
}


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Todo.Database.Entity
{
    [Table("tags")]
    public class TagEntity
    {
        [Key]
        [Column("id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }
        
        [Column("tag")]
        public string Tag { get; set; }
        
        [Column("usage")]
        public int Usage { get; set; }
    }    
}


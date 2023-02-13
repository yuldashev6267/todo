using Microsoft.EntityFrameworkCore;
using Todo.Database.Entity;

namespace Todo.Database
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        
        public DbSet<TodoEntity> Todos { get; set; }
        
        public DbSet<TagEntity> Tags { get; set; }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
            }
        }
    }
}

using Data.Entities;
using Data.EntityConfigs;
using Microsoft.EntityFrameworkCore;

namespace Data;
public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options) 
    {
        
    }
    public virtual DbSet<UserEntity> Users { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureIUniqueEntities();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // for debugging
        //optionsBuilder
        //    .LogTo(Console.WriteLine)
        //    .EnableSensitiveDataLogging()
        //    .EnableDetailedErrors();
    }
}

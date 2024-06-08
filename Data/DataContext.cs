using Data.Entities;
using Data.EntityConfigs;
using Microsoft.EntityFrameworkCore;

namespace Data;
public class DataContext : DbContext
{
    public virtual DbSet<UserEntity> Users { get; set; }
    public virtual DbSet<GeoEntity> Geos { get; set; }
    public virtual DbSet<CompanyEntity> Companies { get; set; }
    public virtual DbSet<AddressEntity> Addresses { get; set; } 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ConfigureIUniqueEntities();
        modelBuilder.ConfigureAddressEntity();
        modelBuilder.ConfigureUserEntity();

        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // TODO: move to Api Program.cs
        optionsBuilder.UseSqlite("Data Source = Database.db");
        // for debugging
        //optionsBuilder
        //    .LogTo(Console.WriteLine)
        //    .EnableSensitiveDataLogging()
        //    .EnableDetailedErrors();
    }
}

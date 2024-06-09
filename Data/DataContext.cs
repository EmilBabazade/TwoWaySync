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
        // for debugging
        //optionsBuilder
        //    .LogTo(Console.WriteLine)
        //    .EnableSensitiveDataLogging()
        //    .EnableDetailedErrors();
    }
}

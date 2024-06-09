using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EntityConfigs;
internal static class AddressEntityConfig
{
    internal static void ConfigureAddressEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AddressEntity>()
            .HasOne(a => a.Geo)
            .WithOne()
            .HasForeignKey(nameof(AddressEntity), nameof(AddressEntity.GeoRowId))
            .OnDelete(DeleteBehavior.Cascade);
    }
}
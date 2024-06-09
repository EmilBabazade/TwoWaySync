using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data.EntityConfigs;
internal static class UniqueEntitiesConfig
{
    internal static void ConfigureIUniqueEntities(this ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if(typeof(IUnique).IsAssignableFrom(entityType.ClrType))
            {
                modelBuilder
                    .Entity(entityType.ClrType)
                    .HasKey(nameof(IUnique.RowId));
            }
        }
    }
}
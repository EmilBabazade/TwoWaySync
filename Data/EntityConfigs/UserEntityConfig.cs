using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.EntityConfigs;
internal static class UserEntityConfig
{
    internal static void ConfigureUserEntity(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserEntity>()
            .HasOne(u => u.Adress)
            .WithOne()
            .HasForeignKey(nameof(AddressEntity), nameof(AddressEntity.UserRowId))
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserEntity>()
            .HasOne(u => u.Company)
            .WithOne()
            .HasForeignKey(nameof(CompanyEntity), nameof(CompanyEntity.UserRowId))
            .OnDelete(DeleteBehavior.Cascade);
    }
}

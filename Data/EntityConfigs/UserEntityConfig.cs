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
            .HasOne(u => u.Address)
            .WithOne()
            .HasForeignKey(nameof(UserEntity), nameof(UserEntity.AddressRowId))
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<UserEntity>()
            .HasOne(u => u.Company)
            .WithOne()
            .HasForeignKey(nameof(UserEntity), nameof(UserEntity.CompanyRowId))
            .OnDelete(DeleteBehavior.Cascade);
    }
}

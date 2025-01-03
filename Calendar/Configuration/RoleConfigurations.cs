
﻿using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Calendar.Configurationn
{
    public class RoleConfiguration : IEntityTypeConfiguration<IdentityRole>
    {
        public void Configure(EntityTypeBuilder<IdentityRole> builder)
        {
            builder.HasData(
                new IdentityRole { Name = UserRoles.CLIENT, NormalizedName = UserRoles.CLIENT },
                new IdentityRole { Name = UserRoles.ADMIN, NormalizedName = UserRoles.ADMIN }
                );
        }
    }
    public static class UserRoles
    {
        public const string CLIENT = "CLIENT";
        public const string ADMIN = "ADMIN";
    }
}

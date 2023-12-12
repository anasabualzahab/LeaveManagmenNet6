using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagmen.Data.Configurations.Entities
{
    public class UserRoleSeedConfiguration : IEntityTypeConfiguration<IdentityUserRole<string>>
    {
        public void Configure(EntityTypeBuilder<IdentityUserRole<string>> builder)
        {
            builder.HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "a123-a123-a123-a123-a123",
                    UserId = "36d4dc5e-82cd-4d3c-9575-0663de71dda0"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "a1223-a1223-a1223-a1223-a1223",
                    UserId = "36d4dc5e-82cd-4d3c-9575-0663de71dd74"
                }


                );
        }
    }
}
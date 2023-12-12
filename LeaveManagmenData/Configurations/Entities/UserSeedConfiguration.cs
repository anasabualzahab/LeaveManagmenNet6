using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LeaveManagmen.Data.Configurations.Entities
{
    public class UserSeedConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            var hasher = new PasswordHasher<Employee>();

            builder.HasData(
                new Employee
                {
                    Id = "36d4dc5e-82cd-4d3c-9575-0663de71dda0",
                    Email = "admin@localhost.com",
                    NormalizedEmail = "ADMIN@LOCALHOST.COM",
                    NormalizedUserName = "ADMIN@LOCALHOST.COM",
                    UserName = "admin@localhost.com",
                    FirstName = "System",
                    LastName = "Admin",
                    PasswordHash = hasher.HashPassword(null, "P@ssw0rd1"),
                    EmailConfirmed = true,

                },
                new Employee
                {
                    Id = "36d4dc5e-82cd-4d3c-9575-0663de71dd74",
                    Email = "user@localhost.com",
                    NormalizedEmail = "USER@LOCALHOST.COM",
                    NormalizedUserName = "USER@LOCALHOST.COM",
                    UserName = "user@localhost.com",
                    FirstName = "System",
                    LastName = "User",
                    PasswordHash = hasher.HashPassword(null, "P@ssw0rd1"),
                    EmailConfirmed = true

                }
                );
        }
    }
}
using Microsoft.AspNetCore.Identity;

namespace LeaveManagmen.Data
{
    public class Employee : IdentityUser
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? TaxId { get; set; }

        public DateTime DateOfBirth { get; set; }

        public DateTime DateOfJoined { get; set; }

    }
}

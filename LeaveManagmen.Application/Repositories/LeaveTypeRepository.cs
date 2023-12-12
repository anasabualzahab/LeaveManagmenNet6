using LeaveManagmen.Data;
using LeaveManagmen.Application.Contracts;
namespace LeaveManagmen.Application.Repositories
{
    public class LeaveTypeRepository : GenericRepository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(ApplicationDbContext context) : base(context)
        {

        }
    }
}

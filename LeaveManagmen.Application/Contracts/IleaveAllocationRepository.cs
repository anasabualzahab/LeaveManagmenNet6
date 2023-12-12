using LeaveManagmen.Data;
using LeaveManagmen.Common.Models;
namespace LeaveManagmen.Application.Contracts
{
    public interface IleaveAllocationRepository : IGenericRepository<LeaveAllocation>
    {

        Task LeaveAllocation(int leaveTypeId);

        Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period);

        Task<EmployeeAllocationVM> GetEmployeeAllocations(string employeeId);
        Task<LeaveAllocation?> GetEmployeeAllocation(string employeeId, int leaveTypeId);

        Task<LeaveAllocationEditVM> GetEmployeeAllocation(int id);
        Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditVM model);


    }
}

using LeaveManagmen.Data;
using LeaveManagmen.Common.Models;
namespace LeaveManagmen.Application.Contracts
{
    public interface IleaveRequestRepository : IGenericRepository<LeaveRequest>
    {
        Task<bool> CreateLeaveRequest(LeaveRequestCreateVM model);
        Task<EmployeeLeaveRequestViewVM> GetMyLeaveDetails();
        Task<List<LeaveRequestVM>> GetAsyncAll(string employeeId);
        Task ChangeApprovalStatus(int leaveRequestId, bool approved);
        Task<AdminLeaveReqeustViewVM> GetAdminLeaveRequestList();
        Task<LeaveRequestVM?> GetLeaveRequestAsync(int? id);
        Task CancelLeaveRequest(int leaveReauestId);

    }
}

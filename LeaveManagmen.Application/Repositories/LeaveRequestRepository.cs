using AutoMapper;
using AutoMapper.QueryableExtensions;
using LeaveManagmen.Data;
using LeaveManagmen.Application.Contracts;
using LeaveManagmen.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace LeaveManagmen.Application.Repositories
{
    public class LeaveRequestRepository : GenericRepository<LeaveRequest>, IleaveRequestRepository
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IleaveAllocationRepository leaveAllocationRepository;
        private readonly UserManager<Employee> userManager;
        private readonly IEmailSender emailSender;
        private readonly AutoMapper.IConfigurationProvider configurationProvider;

        public LeaveRequestRepository(ApplicationDbContext context, IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IleaveAllocationRepository leaveAllocationRepository,
            UserManager<Employee> userManager,
            IEmailSender emailSender,
            AutoMapper.IConfigurationProvider configurationProvider) : base(context)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContextAccessor = httpContextAccessor;
            this.leaveAllocationRepository = leaveAllocationRepository;
            this.userManager = userManager;
            this.emailSender = emailSender;
            this.configurationProvider = configurationProvider;
        }

        public async Task<bool> CreateLeaveRequest(LeaveRequestCreateVM model)
        {
            var user = await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);

            var leaveAllocation = await leaveAllocationRepository.GetEmployeeAllocation(user.Id, model.LeaveTypeId);

            if (leaveAllocation == null)
            {
                return false;
            }

            int daysRequested = (int)(model.EndDate.Value - model.StartDate.Value).TotalDays;

            if (daysRequested > leaveAllocation.NumberOfDays)
            {
                return false;
            }

            var leaveRequest = mapper.Map<LeaveRequest>(model);
            leaveRequest.DateRequested = DateTime.Now;
            leaveRequest.RequestingEmployeeId = user.Id;

            await AddAsync(leaveRequest);

            await emailSender.SendEmailAsync(user.Email, "Leave Request Submitted Successfully", $"Your leave request from " +
                $"{leaveRequest.StartDate} to {leaveRequest.EndDate} has been submitted for approval");

            return true;

        }




        public async Task<EmployeeLeaveRequestViewVM> GetMyLeaveDetails()
        {
            var user = await userManager.GetUserAsync(httpContextAccessor?.HttpContext?.User);
            var allocation = (await leaveAllocationRepository.GetEmployeeAllocations(user.Id)).LeaveAllocations;

            var request = await GetAsyncAll(user.Id);

            var model = new EmployeeLeaveRequestViewVM(allocation, request);

            return model;

        }

        public async Task<AdminLeaveReqeustViewVM> GetAdminLeaveRequestList()
        {
            var leaveRequests = await context.LeaveRequests.Include(q => q.LeaveType).ToListAsync();
            var model = new AdminLeaveReqeustViewVM
            {
                TotalRequests = leaveRequests.Count,
                ApprovedRequests = leaveRequests.Count(q => q.Approved == true),
                PendingRequests = leaveRequests.Count(q => q.Approved == null),
                RejectedRequests = leaveRequests.Count(q => q.Approved == false),
                LeaveRequest = mapper.Map<List<LeaveRequestVM>>(leaveRequests),
            };

            foreach (var leaveRequest in model.LeaveRequest)
            {
                leaveRequest.Employee = mapper.Map<EmployeeListVM>(await userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId));
            }

            return model;

        }

        public async Task ChangeApprovalStatus(int leaveRequestId, bool approved)
        {
            var leaveRequest = await GetAsync(leaveRequestId);
            leaveRequest.Approved = approved;

            if (approved)
            {
                var allocation = await leaveAllocationRepository.GetEmployeeAllocation(leaveRequest.RequestingEmployeeId, leaveRequest.LeaveTypeId);
                int daysRequested = (int)(leaveRequest.EndDate - leaveRequest.StartDate).TotalDays;
                allocation.NumberOfDays -= daysRequested;

                await leaveAllocationRepository.UpdateAsync(allocation);
            }

            await UpdateAsync(leaveRequest);

            var user = await userManager.FindByIdAsync(leaveRequest.RequestingEmployeeId);
            var approvalStatus = approved ? "Approved" : "Declined";

            await emailSender.SendEmailAsync(user.Email, $"Leave Request {approvalStatus}", $"Your leave request from " +
                $"{leaveRequest.StartDate} to {leaveRequest.EndDate} has been {approvalStatus}");

        }
        public async Task<LeaveRequestVM?> GetLeaveRequestAsync(int? id)
        {
            var leaveRequest = await context.LeaveRequests
                .Include(q => q.LeaveType)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (leaveRequest == null)
            {
                return null;
            }

            var model = mapper.Map<LeaveRequestVM>(leaveRequest);
            model.Employee = mapper.Map<EmployeeListVM>(await userManager.FindByIdAsync(leaveRequest?.RequestingEmployeeId));
            return model;
        }
        public async Task<List<LeaveRequestVM>> GetAsyncAll(string employeeId)
        {
            return await context.LeaveRequests.Where(q => q.RequestingEmployeeId == employeeId)
                .ProjectTo<LeaveRequestVM>(configurationProvider)
                .ToListAsync();
        }

        public async Task CancelLeaveRequest(int leaveReauestId)
        {
            var leaveRequest = await GetAsync(leaveReauestId);
            leaveRequest.Cancelled = true;
            await UpdateAsync(leaveRequest);
        }
    }
}

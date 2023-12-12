﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
using LeaveManagmen.Application.Repositories;
using LeaveManagmen.Common.Constants;
using LeaveManagmen.Data;
using LeaveManagmen.Application.Contracts;
using LeaveManagmen.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
namespace LeaveManagmen.Application.Repositories
{
    public class LeaveAllocationRepository : GenericRepository<LeaveAllocation>, IleaveAllocationRepository
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<Employee> userManager;
        private readonly ILeaveTypeRepository leaveTypeRepository;
        private readonly IMapper mapper;
        private readonly AutoMapper.IConfigurationProvider configurationProvider;

        public LeaveAllocationRepository(ApplicationDbContext context,
            UserManager<Employee> userManager,
            ILeaveTypeRepository leaveTypeRepository,
            IMapper mapper,
            AutoMapper.IConfigurationProvider configurationProvider) : base(context)
        {
            this.context = context;
            this.userManager = userManager;
            this.leaveTypeRepository = leaveTypeRepository;
            this.mapper = mapper;
            this.configurationProvider = configurationProvider;
        }

        public async Task<bool> AllocationExists(string employeeId, int leaveTypeId, int period)
        {
            return await context.LeaveAllocations.AnyAsync(q => q.EmployeeId == employeeId &&
                                                            q.LeaveTypeId == leaveTypeId &&
                                                            q.Period == period);
        }

        public async Task<EmployeeAllocationVM> GetEmployeeAllocations(string employeeId)
        {
            var allocations = await context.LeaveAllocations
                .Include(q => q.LeaveType)
                .Where(q => q.EmployeeId == employeeId)
                .ProjectTo<LeaveAllocationVM>(configurationProvider)
                .ToListAsync();

            var employee = await userManager.FindByIdAsync(employeeId);

            var employeeAllocationModel = mapper.Map<EmployeeAllocationVM>(employee);
            employeeAllocationModel.LeaveAllocations = allocations; /*= mapper.Map<List<LeaveAllocationVM>>(allocations);*/

            return employeeAllocationModel;
        }

        public async Task<LeaveAllocationEditVM> GetEmployeeAllocation(int id)
        {
            var allocation = await context.LeaveAllocations
                .Include(q => q.LeaveType)
                .ProjectTo<LeaveAllocationEditVM>(configurationProvider)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (allocation == null)
            {
                return null;
            }

            var employee = await userManager.FindByIdAsync(allocation.EmployeeId);

            var model = mapper.Map<LeaveAllocationEditVM>(allocation);
            model.Employee = mapper.Map<EmployeeListVM>(await userManager.FindByIdAsync(allocation.EmployeeId));

            return model;
        }

        public async Task LeaveAllocation(int leaveTypeId)
        {
            var employees = await userManager.GetUsersInRoleAsync(Roles.User);
            var period = DateTime.Now.Year;
            var leaveType = await leaveTypeRepository.GetAsync(leaveTypeId);

            var allocations = new List<LeaveAllocation>();

            foreach (var employee in employees)
            {
                if (await AllocationExists(employee.Id, leaveTypeId, period))
                    continue;

                allocations.Add(new LeaveAllocation
                {
                    EmployeeId = employee.Id,
                    LeaveTypeId = leaveTypeId,
                    Period = period,
                    NumberOfDays = leaveType.DefaultDays
                });


            }
            await AddRangeAsync(allocations);


        }



        public async Task<LeaveAllocation?> GetEmployeeAllocation(string employeeId, int leaveTypeId)
        {
            return await context.LeaveAllocations.
                FirstOrDefaultAsync(q => q.EmployeeId == employeeId && q.LeaveTypeId == leaveTypeId);
        }

        public async Task<bool> UpdateEmployeeAllocation(LeaveAllocationEditVM model)
        {
            var leaveAllocaiton = await GetAsync(model.Id);
            if (leaveAllocaiton == null)
            {
                return false;

            }
            leaveAllocaiton.Period = model.Period;
            leaveAllocaiton.NumberOfDays = model.NumberOfDays;
            await UpdateAsync(leaveAllocaiton);
            return true;
        }
    }
}
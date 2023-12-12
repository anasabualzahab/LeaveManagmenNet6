using AutoMapper;
using LeaveManagmen.Common.Constants;
using LeaveManagmen.Data;
using LeaveManagmen.Application.Contracts;
using LeaveManagmen.Common.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace LeaveManagmen.Web.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        private readonly IMapper mapper;
        private readonly IleaveAllocationRepository ileaveAllocationRepository;
        private readonly ILeaveTypeRepository leaveTypeRepository;

        public EmployeesController(UserManager<Employee> userManager
            , IMapper mapper
            , IleaveAllocationRepository ileaveAllocationRepository
            , ILeaveTypeRepository leaveTypeRepository)
        {
            _userManager = userManager;
            this.mapper = mapper;
            this.ileaveAllocationRepository = ileaveAllocationRepository;
            this.leaveTypeRepository = leaveTypeRepository;
        }
        // GET: EmployeesController
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.GetUsersInRoleAsync(Roles.User);
            var model = mapper.Map<List<EmployeeListVM>>(employees);
            return View(model);
        }

        // GET: EmployeesController/ViewAllocations/5
        public async Task<ActionResult> ViewAllocations(string id)
        {
            var model = await ileaveAllocationRepository.GetEmployeeAllocations(id);
            return View(model);
        }

        // GET: EmployeesController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: EmployeesController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: EmployeesController/Edit/5
        public async Task<ActionResult> EditAllocation(int id)
        {
            var model = await ileaveAllocationRepository.GetEmployeeAllocation(id);
            if (model == null)
            {
                return NotFound();
            }
            return View(model);
        }

        // POST: EmployeesController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAllocation(int id, LeaveAllocationEditVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (await ileaveAllocationRepository.UpdateEmployeeAllocation(model))
                    {
                        return RedirectToAction(nameof(ViewAllocations), new { id = model.EmployeeId });

                    }

                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "An Error has occurred, Please try again later.");
            }
            model.Employee = mapper.Map<EmployeeListVM>(await _userManager.FindByIdAsync(model.EmployeeId));
            model.LeaveType = mapper.Map<LeaveTypeVM>(await leaveTypeRepository.GetAsync(model.LeaveTypeId));
            return View(model);
        }

        // GET: EmployeesController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: EmployeesController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}

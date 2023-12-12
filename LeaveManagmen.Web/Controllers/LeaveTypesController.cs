using AutoMapper;
using LeaveManagmen.Data;
using LeaveManagmen.Application.Contracts;
using LeaveManagmen.Common.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagmen.Web.Controllers
{
    public class LeaveTypesController : Controller
    {
        ILeaveTypeRepository _leaveTypeRepository;
        private readonly IMapper mapper;
        private readonly IleaveAllocationRepository leaveAllocationRepository;

        public LeaveTypesController(ILeaveTypeRepository leaveTypeRepository, IMapper mapper,
            IleaveAllocationRepository leaveAllocationRepository)
        {
            _leaveTypeRepository = leaveTypeRepository;
            this.mapper = mapper;
            this.leaveAllocationRepository = leaveAllocationRepository;
        }

        // GET: LeaveTypes
        public async Task<IActionResult> Index()
        {

            var leaveTypes = mapper.Map<List<LeaveTypeVM>>(await _leaveTypeRepository.GetAllAsync());

            return View(leaveTypes);
        }

        // GET: LeaveTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {

            var leaveType = await _leaveTypeRepository.GetAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            var leaveTypeVM = mapper.Map<LeaveTypeVM>(leaveType);

            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveTypeVM leaveTypeVM)
        {
            if (ModelState.IsValid)
            {
                var leaveType = mapper.Map<LeaveType>(leaveTypeVM);
                await _leaveTypeRepository.AddAsync(leaveType);
                return RedirectToAction(nameof(Index));
            }
            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var leaveType = await _leaveTypeRepository.GetAsync(id);
            if (leaveType == null)
            {
                return NotFound();
            }
            var leaveTypeVM = mapper.Map<LeaveTypeVM>(leaveType);

            return View(leaveTypeVM);
        }

        // POST: LeaveTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, LeaveTypeVM leaveTypeVM)
        {
            if (id != leaveTypeVM.Id)
            {
                return NotFound();
            }
            var leaveType = await _leaveTypeRepository.GetAsync(id);

            if (leaveType == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {

                    leaveType = await _leaveTypeRepository.GetAsync(id);
                    mapper.Map(leaveTypeVM, leaveType);
                    await _leaveTypeRepository.UpdateAsync(leaveType);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await LeaveTypeExists(leaveTypeVM.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(leaveTypeVM);
        }

        // GET: LeaveTypes/Delete/5


        // POST: LeaveTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _leaveTypeRepository.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> LeaveTypeExists(int id)
        {
            return await _leaveTypeRepository.Exists(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AllocateLeave(int id)
        {
            await leaveAllocationRepository.LeaveAllocation(id);
            return RedirectToAction(nameof(Index));
        }
    }
}

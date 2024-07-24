using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VerlofApp.Data;
using VerlofApp.Models;

namespace VerlofApp.Controllers
{
    public class LeaveRequestsController : Controller
    {
        private readonly Context _context;

        public LeaveRequestsController(Context context)
        {
            _context = context;
        }

        // GET: LeaveRequests
        public async Task<IActionResult> Index()
        {
            return View(await _context.LeaveRequests.ToListAsync());
        }

        // GET: LeaveRequests/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(m => m.LeaveId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // GET: LeaveRequests/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LeaveRequests/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LeaveId,EmployeeId,EmployeeName,StartDate,EndDate,Reason")] LeaveRequest leaveRequest)
        {
            // No overlap allowed            
            if (NoOverlap(leaveRequest.EmployeeId, leaveRequest.StartDate, leaveRequest.EndDate))
            {
                ModelState.AddModelError("", "Overlap is not allowed.");
                return View(leaveRequest);
            };

            // No weekend registration required
            if (IsWeekend(leaveRequest.StartDate) || IsWeekend(leaveRequest.EndDate))
            {
                ModelState.AddModelError("", "Weekend leave is not required for registration.");
                return View(leaveRequest);
            }

            if (ModelState.IsValid)
            {
                _context.Add(leaveRequest);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(leaveRequest);
        }

        // GET: LeaveRequests/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest == null)
            {
                return NotFound();
            }
            return View(leaveRequest);
        }

        // POST: LeaveRequests/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LeaveId,EmployeeId,EmployeeName,StartDate,EndDate,Reason")] LeaveRequest leaveRequest)
        {
            if (id != leaveRequest.LeaveId)
            {
                return NotFound();
            }

            // No Overlap
            if (NoOverlap(leaveRequest.EmployeeId, leaveRequest.StartDate, leaveRequest.EndDate))
            {
                ModelState.AddModelError("", "Overlap is not allowed.");
                return View(leaveRequest);
            };

            // No weekend registration required
            if (IsWeekend(leaveRequest.StartDate) || IsWeekend(leaveRequest.EndDate))
            {
                ModelState.AddModelError("", "Weekend leave is not required for registration.");
                return View(leaveRequest);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(leaveRequest);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LeaveRequestExists(leaveRequest.LeaveId))
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
            return View(leaveRequest);
        }

        // GET: LeaveRequests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var leaveRequest = await _context.LeaveRequests
                .FirstOrDefaultAsync(m => m.LeaveId == id);
            if (leaveRequest == null)
            {
                return NotFound();
            }

            return View(leaveRequest);
        }

        // POST: LeaveRequests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var leaveRequest = await _context.LeaveRequests.FindAsync(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LeaveRequestExists(int id)
        {
            return _context.LeaveRequests.Any(e => e.LeaveId == id);
        }

        private bool IsWeekend(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday;
        }

        private bool NoOverlap(int id, DateTime startDate, DateTime endDate)
        {
            return _context.LeaveRequests.Any(l => l.EmployeeId == id && l.StartDate < endDate && l.EndDate > startDate);
        }
    }
}

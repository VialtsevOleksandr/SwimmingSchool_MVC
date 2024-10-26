using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SwimmingSchool_MVC.Models;

namespace SwimmingSchool_MVC.Controllers
{
    public class GroupSchedulesController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupSchedulesController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: GroupSchedules
        public async Task<IActionResult> Index()
        {
            var dbSwimmingSchoolContext = _context.GroupSchedules.Include(g => g.Day).Include(g => g.Group);
            return View(await dbSwimmingSchoolContext.ToListAsync());
        }

        // GET: GroupSchedules/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupSchedule = await _context.GroupSchedules
                .Include(g => g.Day)
                .Include(g => g.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupSchedule == null)
            {
                return NotFound();
            }

            return View(groupSchedule);
        }

        // GET: GroupSchedules/Create
        public IActionResult Create(int? groupId)
        {
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay");
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupId);
            return View();
        }

        // POST: GroupSchedules/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupId,DayId,TimeOfTrainingStart,TrainingTime")] GroupSchedule groupSchedule)
        {
            if (_context.GroupSchedules.Any(g => g.GroupId == groupSchedule.GroupId && g.DayId == groupSchedule.DayId))
            {
                ModelState.AddModelError("DayId", "Група не може мати більше тренувань у цей день");
                ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
                return View(groupSchedule);
            }
            if (groupSchedule.TrainingTime <= 0)
            {
                ModelState.AddModelError("TrainingTime", "Тривалість тренування повинна бути більше 0");
                ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
                return View(groupSchedule);
            }
            if (ModelState.IsValid)
            {
                _context.Add(groupSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
            return View(groupSchedule);
        }

        // GET: GroupSchedules/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupSchedule = await _context.GroupSchedules.FindAsync(id);
            if (groupSchedule == null)
            {
                return NotFound();
            }
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
            return View(groupSchedule);
        }

        // POST: GroupSchedules/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupId,DayId,TimeOfTrainingStart,TrainingTime")] GroupSchedule groupSchedule)
        {
            if (id != groupSchedule.Id)
            {
                return NotFound();
            }
            if (_context.GroupSchedules.Any(g => g.GroupId == groupSchedule.GroupId && g.DayId == groupSchedule.DayId))
            {
                ModelState.AddModelError("DayId", "Група не може мати більше тренувань у цей день");
                ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
                return View(groupSchedule);
            }
            if (groupSchedule.TrainingTime <= 0)
            {
                ModelState.AddModelError("TrainingTime", "Тривалість тренування повинна бути більше 0");
                ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
                ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
                return View(groupSchedule);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupScheduleExists(groupSchedule.Id))
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
            ViewData["DayId"] = new SelectList(_context.Days, "Id", "NameOfDay", groupSchedule.DayId);
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", groupSchedule.GroupId);
            return View(groupSchedule);
        }

        // GET: GroupSchedules/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupSchedule = await _context.GroupSchedules
                .Include(g => g.Day)
                .Include(g => g.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupSchedule == null)
            {
                return NotFound();
            }

            return View(groupSchedule);
        }

        // POST: GroupSchedules/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupSchedule = await _context.GroupSchedules.FindAsync(id);
            if (groupSchedule != null)
            {
                _context.GroupSchedules.Remove(groupSchedule);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupScheduleExists(int id)
        {
            return _context.GroupSchedules.Any(e => e.Id == id);
        }
    }
}

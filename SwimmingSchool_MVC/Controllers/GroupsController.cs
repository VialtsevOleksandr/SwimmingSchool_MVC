using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.DependencyResolver;
using SwimmingSchool_MVC.Models;

namespace SwimmingSchool_MVC.Controllers
{
    public class GroupsController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupsController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: Groups
        public async Task<IActionResult> Index()
        {
            var dbSwimmingSchoolContext = _context.Groups.Include(g => g.GroupType).Include(g => g.Trainer);
            return View(await dbSwimmingSchoolContext.ToListAsync());
        }

        // GET: Groups/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .Include(g => g.GroupType)
                .Include(g => g.Trainer)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // GET: Groups/Create
        public IActionResult Create(int? trainerId)
        {
            ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name");
            ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
            }), "Id", "FullName", trainerId);
            return View();
        }

        // POST: Groups/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,GroupName,TrainerId,GroupTypeId")] Group group)
        {
            if (_context.Groups.Any(g => g.GroupName == group.GroupName && g.TrainerId == group.TrainerId))
            {
                ModelState.AddModelError("GroupName", "Група з таким ім'ям та тренером вже існує");
                ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name", group.GroupTypeId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
                {
                    Id = t.Id,
                    FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
                }), "Id", "FullName", group.TrainerId);
                return View(group);
            }

            if (ModelState.IsValid)
            {
                _context.Add(group);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name", group.GroupTypeId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
            }), "Id", "FullName", group.TrainerId);
            return View(group);
        }

        // GET: Groups/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups.FindAsync(id);
            if (group == null)
            {
                return NotFound();
            }
            ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name", group.GroupTypeId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
            }), "Id", "FullName", group.TrainerId);
            return View(group);
        }

        // POST: Groups/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GroupName,TrainerId,GroupTypeId")] Group group)
        {
            if (id != group.Id)
            {
                return NotFound();
            }
            if (_context.Groups.Any(g => g.GroupName == group.GroupName && g.TrainerId == group.TrainerId))
            {
                ModelState.AddModelError("GroupName", "Група з таким ім'ям та тренером вже існує");
                ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name", group.GroupTypeId);
                ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
                {
                    Id = t.Id,
                    FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
                }), "Id", "FullName", group.TrainerId);
                return View(group);
            }
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(group);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupExists(group.Id))
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
            ViewData["GroupTypeId"] = new SelectList(_context.GroupTypes, "Id", "Name", group.GroupTypeId);
            ViewData["TrainerId"] = new SelectList(_context.Trainers.Select(t => new
            {
                Id = t.Id,
                FullName = t.LastName + " " + t.FirstName + " " + t.MiddleName
            }), "Id", "FullName", group.TrainerId);
            return View(group);
        }

        // GET: Groups/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var group = await _context.Groups
                .Include(g => g.Trainer)
                .Include(g => g.GroupType)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (group == null)
            {
                return NotFound();
            }

            return View(group);
        }

        // POST: Groups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var group = await _context.Groups
            .Include(g => g.Trainer)
            .Include(g => g.GroupType)
            .FirstOrDefaultAsync(m => m.Id == id);

            if (await _context.Pupils.AnyAsync(p => p.GroupId == group.Id))
            {
                ViewBag.ShowCannotDeleteModal = true;

                return View(group);
            }
            if (group != null)
            {
                _context.Groups.Remove(group);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}

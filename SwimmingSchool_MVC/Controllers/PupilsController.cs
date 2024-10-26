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
    public class PupilsController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public PupilsController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: Pupils
        public async Task<IActionResult> Index()
        {
            var dbSwimmingSchoolContext = _context.Pupils.Include(p => p.Group);
            return View(await dbSwimmingSchoolContext.ToListAsync());
        }

        // GET: Pupils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pupil = await _context.Pupils
                .Include(p => p.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pupil == null)
            {
                return NotFound();
            }

            return View(pupil);
        }

        // GET: Pupils/Create
        public IActionResult Create()
        {
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName");
            return View();
        }

        // POST: Pupils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,MiddleName,Birthday,ParentsPhoneNumber,GroupId")] Pupil pupil)
        {
            if (ModelState.IsValid)
            {
                if (pupil.Birthday > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("Birthday", "Дата народження не може бути датою в майбутньому");
                    ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
                    return View(pupil);
                }
                if (pupil.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-19)) || pupil.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-4)))
                {
                    ModelState.AddModelError("Birthday", "Учень не може бути молодшим 5 років або старшим за 18");
                    ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
                    return View(pupil);
                }
                _context.Add(pupil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
            return View(pupil);
        }

        // GET: Pupils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pupil = await _context.Pupils.FindAsync(id);
            if (pupil == null)
            {
                return NotFound();
            }
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
            return View(pupil);
        }

        // POST: Pupils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,MiddleName,Birthday,ParentsPhoneNumber,GroupId")] Pupil pupil)
        {
            if (id != pupil.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (pupil.Birthday > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("Birthday", "Дата народження не може бути датою в майбутньому");
                    ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
                    return View(pupil);
                }
                if (pupil.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-18)) || pupil.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-5)))
                {
                    ModelState.AddModelError("Birthday", "Учень не може бути молодшим 5 років або старшим за 18");
                    ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
                    return View(pupil);
                }
                try
                {
                    _context.Update(pupil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PupilExists(pupil.Id))
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
            ViewData["GroupId"] = new SelectList(_context.Groups, "Id", "GroupName", pupil.GroupId);
            return View(pupil);
        }

        // GET: Pupils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pupil = await _context.Pupils
                .Include(p => p.Group)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pupil == null)
            {
                return NotFound();
            }

            // Підрахунок кількості записів у PupilsEvents для цього учня
            var relatedPupilsEventsCount = _context.PupilsEvents.Count(pe => pe.PupilsId == id);
            ViewBag.RelatedPupilsEventsCount = relatedPupilsEventsCount;
            return View(pupil);
        }

        // POST: Pupils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pupil = await _context.Pupils.FindAsync(id);
            if (pupil != null)
            {
                // Видалення записів з PupilsEvents
                var relatedPupilsEvents = _context.PupilsEvents.Where(pe => pe.PupilsId == id);
                _context.PupilsEvents.RemoveRange(relatedPupilsEvents);

                _context.Pupils.Remove(pupil);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PupilExists(int id)
        {
            return _context.Pupils.Any(e => e.Id == id);
        }
    }
}

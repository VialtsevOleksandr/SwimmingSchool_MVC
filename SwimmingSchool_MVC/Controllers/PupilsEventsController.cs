using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SwimmingSchool_MVC.Models;

namespace SwimmingSchool_MVC.Controllers
{
    public class PupilsEventsController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public PupilsEventsController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: PupilsEvents
        public async Task<IActionResult> Index()
        {
            var dbSwimmingSchoolContext = _context.PupilsEvents.Include(p => p.Event).Include(p => p.Pupils);
            return View(await dbSwimmingSchoolContext.ToListAsync());
        }
        private int GetRegisteredPupilsCount(int eventId)
        {
            return _context.PupilsEvents.Count(pe => pe.EventId == eventId);
        }

        // GET: PupilsEvents/Details/5
        public async Task<IActionResult> Details(int? pupilsId, int? eventId)
        {
            if (pupilsId == null || eventId == null)
            {
                return NotFound();
            }

            var pupilsEvent = await _context.PupilsEvents
                .Include(p => p.Event)
                .Include(p => p.Pupils)
                .FirstOrDefaultAsync(m => m.PupilsId == pupilsId && m.EventId == eventId);
            if (pupilsEvent == null)
            {
                return NotFound();
            }

            return View(pupilsEvent);
        }

        // GET: PupilsEvents/Create
        public IActionResult Create(int? eventId)
        {
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", eventId);
            ViewData["PupilsId"] = new SelectList(_context.Pupils.Select(p => new
            {
                Id = p.Id,
                FullName = p.LastName + " " + p.FirstName + " " + p.MiddleName
            }), "Id", "FullName");
            return View();
        }

        // POST: PupilsEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int[] PupilsId, [Bind("EventId,Info,Result")] PupilsEvent pupilsEvent)
        {
            var evt = await _context.Events.FindAsync(pupilsEvent.EventId);
            if (evt == null)
            {
                return NotFound();
            }

            var currentRegisteredPupilsCount = GetRegisteredPupilsCount(pupilsEvent.EventId);
            var newRegistrationsCount = PupilsId.Length;

            if (currentRegisteredPupilsCount + newRegistrationsCount > evt.MaxPupilsAmount)
            {
                TempData["ErrorMessage"] = $"Не можна додати більше учнів, ніж вказано в максимальній кількості учасників.";
                return RedirectToAction("Details", "Events", new { id = pupilsEvent.EventId });
            }

            foreach (var pupilId in PupilsId)
            {
                if (_context.PupilsEvents.Any(pe => pe.PupilsId == pupilId && pe.EventId == pupilsEvent.EventId))
                {
                    TempData["ErrorMessage"] = $"Учень з ID {pupilId} вже зареєстрований на цю подію.";
                    continue;
                }

                var newPupilEvent = new PupilsEvent
                {
                    PupilsId = pupilId, 
                    EventId = pupilsEvent.EventId,
                    Info = string.IsNullOrEmpty(pupilsEvent.Info) ? "Зареєстрований" : pupilsEvent.Info,
                    Result = pupilsEvent.Result
                };

                _context.Add(newPupilEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", new { id = pupilsEvent.EventId });
        }

        // GET: PupilsEvents/Edit/5
        public async Task<IActionResult> Edit(int? pupilsId, int? eventId)
        {
            if (pupilsId == null || eventId == null)
            {
                return NotFound();
            }

            var pupilsEvent = await _context.PupilsEvents.FindAsync(pupilsId, eventId);
            if (pupilsEvent == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", pupilsEvent.EventId);
            ViewData["PupilsId"] = new SelectList(_context.Pupils.Select(p => new
            {
                Id = p.Id,
                FullName = p.LastName + " " + p.FirstName + " " + p.MiddleName
            }), "Id", "FullName", pupilsEvent.PupilsId);
            return View(pupilsEvent);
        }

        // POST: PupilsEvents/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int pupilsId, int eventId, [Bind("PupilsId,EventId,Info,Result")] PupilsEvent pupilsEvent)
        {
            if (pupilsId != pupilsEvent.PupilsId || eventId != pupilsEvent.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pupilsEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PupilsEventExists(pupilsEvent.PupilsId, pupilsEvent.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Events", new { id = pupilsEvent.EventId });
            }
            ViewData["EventId"] = new SelectList(_context.Events, "Id", "Name", pupilsEvent.EventId);
            ViewData["PupilsId"] = new SelectList(_context.Pupils.Select(p => new
            {
                Id = p.Id,
                FullName = p.LastName + " " + p.FirstName + " " + p.MiddleName
            }), "Id", "FullName", pupilsEvent.PupilsId);
            return View(pupilsEvent);
        }

        // GET: PupilsEvents/Delete/5
        public async Task<IActionResult> Delete(int? pupilsId, int? eventId)
        {
            if (pupilsId == null || eventId == null)
            {
                return NotFound();
            }

            var pupilsEvent = await _context.PupilsEvents
                .Include(p => p.Event)
                .Include(p => p.Pupils)
                .FirstOrDefaultAsync(m => m.PupilsId == pupilsId && m.EventId == eventId);
            if (pupilsEvent == null)
            {
                return NotFound();
            }

            return View(pupilsEvent);
        }

        // POST: PupilsEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int pupilsId, int eventId)
        {
            var pupilsEvent = await _context.PupilsEvents.FindAsync(pupilsId, eventId);
            _context.PupilsEvents.Remove(pupilsEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Events", new { id = pupilsEvent.EventId });
        }

        private bool PupilsEventExists(int pupilsId, int eventId)
        {
            return _context.PupilsEvents.Any(e => e.PupilsId == pupilsId && e.EventId == eventId);
        }
    }
}

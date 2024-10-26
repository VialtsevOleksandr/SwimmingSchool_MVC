﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SwimmingSchool_MVC.Models;

namespace SwimmingSchool_MVC.Controllers
{
    public class TrainersController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public TrainersController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: Trainers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Trainers.ToListAsync());
        }

        // GET: Trainers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // GET: Trainers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Trainers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,LastName,FirstName,MiddleName,Birthday,PhoneNumber,Email")] Trainer trainer)
        {
            if (ModelState.IsValid)
            {
                if (trainer.Birthday > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("Birthday", "Дата народження не може бути датою в майбутньому");
                    return View(trainer);
                }
                if (trainer.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-75)) || trainer.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
                {
                    ModelState.AddModelError("Birthday", "Вчитель не може бути молодшим 16 років або старшим за 75");
                    return View(trainer);
                }
                _context.Add(trainer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(trainer);
        }

        // GET: Trainers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }
            return View(trainer);
        }

        // POST: Trainers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LastName,FirstName,MiddleName,Birthday,PhoneNumber,Email")] Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (trainer.Birthday > DateOnly.FromDateTime(DateTime.Today))
                {
                    ModelState.AddModelError("Birthday", "Дата народження не може бути датою в майбутньому");
                    return View(trainer);
                }
                if (trainer.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-75)) || trainer.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
                {
                    ModelState.AddModelError("Birthday", "Вчитель не може бути молодшим 16 років або старшим за 75");
                    return View(trainer);
                }
                try
                {
                    _context.Update(trainer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TrainerExists(trainer.Id))
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
            return View(trainer);
        }

        // GET: Trainers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var trainer = await _context.Trainers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (trainer == null)
            {
                return NotFound();
            }

            return View(trainer);
        }

        // POST: Trainers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (await _context.Groups.AnyAsync(g => g.TrainerId == trainer.Id))
            {
                ViewBag.ShowCannotDeleteModal = true;
                return View(trainer);
            }
            if (trainer != null)
            {
                _context.Trainers.Remove(trainer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}

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
    public class GroupTypesController : Controller
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupTypesController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: GroupTypes
        public async Task<IActionResult> Index()
        {
            return View(await _context.GroupTypes.ToListAsync());
        }

        // GET: GroupTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupType == null)
            {
                return NotFound();
            }

            return View(groupType);
        }

        // GET: GroupTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GroupTypes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] GroupType groupType)
        {
            if (_context.GroupTypes.Any(g => g.Name == groupType.Name))
            {
                ModelState.AddModelError("Name", "Такий тип групи вже існує");
                return View(groupType);
            }
            if (ModelState.IsValid)
            {
                _context.Add(groupType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(groupType);
        }

        // GET: GroupTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes.FindAsync(id);
            if (groupType == null)
            {
                return NotFound();
            }
            return View(groupType);
        }

        // POST: GroupTypes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] GroupType groupType)
        {
            if (id != groupType.Id)
            {
                return NotFound();
            }
            if (_context.GroupTypes.Any(g => g.Name == groupType.Name))
            {
                ModelState.AddModelError("Name", "Такий тип групи вже існує");
                return View(groupType);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(groupType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GroupTypeExists(groupType.Id))
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
            return View(groupType);
        }

        // GET: GroupTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var groupType = await _context.GroupTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (groupType == null)
            {
                return NotFound();
            }

            return View(groupType);
        }

        // POST: GroupTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var groupType = await _context.GroupTypes.FindAsync(id);
            if (await _context.Groups.AnyAsync(g => g.GroupTypeId == groupType.Id))
            {
                ViewBag.ShowCannotDeleteModal = true;

                return View(groupType);
            }
            if (groupType != null)
            {
                _context.GroupTypes.Remove(groupType);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GroupTypeExists(int id)
        {
            return _context.GroupTypes.Any(e => e.Id == id);
        }
    }
}

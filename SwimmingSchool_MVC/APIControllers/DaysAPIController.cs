using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SwimmingSchool_MVC.Models;

namespace SwimmingSchool_MVC.APIControllers
{
    [Route("[controller]")]
    [ApiController]
    public class DaysAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public DaysAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/DaysAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Day>>> GetDays()
        {
            return await _context.Days.ToListAsync();
        }

        // GET: api/DaysAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Day>> GetDay(int id)
        {
            var day = await _context.Days.FindAsync(id);

            if (day == null)
            {
                return NotFound();
            }

            return day;
        }

        // PUT: api/DaysAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDay(int id, Day day)
        {
            if (id != day.Id)
            {
                return BadRequest();
            }
            if (_context.Days.Any(d => d.NameOfDay == day.NameOfDay))
            {
                return BadRequest(new { message = "День з таким ім'ям вже існує" });
            }

            _context.Entry(day).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DayExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DaysAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Day>> PostDay(Day day)
        {
            if (_context.Days.Any(d => d.NameOfDay == day.NameOfDay))
            {
                return BadRequest(new { message = "День з таким ім'ям вже існує" });
            }
            _context.Days.Add(day);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDay", new { id = day.Id }, day);
        }

        // DELETE: api/DaysAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDay(int id)
        {
            var day = await _context.Days.FindAsync(id);
            if (day == null)
            {
                return NotFound();
            }
            if (await _context.GroupSchedules.AnyAsync(gs => gs.DayId == day.Id))
            {
                return BadRequest(new { message = "Не можна видалити день, який пов'язаний з розкладом групи" });
            }
            _context.Days.Remove(day);
            await _context.SaveChangesAsync();

            return Ok(new { message = "День успішно видалено" });
        }

        private bool DayExists(int id)
        {
            return _context.Days.Any(e => e.Id == id);
        }
    }
}

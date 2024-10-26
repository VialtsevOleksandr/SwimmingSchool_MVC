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
    public class PupilsAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public PupilsAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/PupilsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pupil>>> GetPupils()
        {
            return await _context.Pupils.ToListAsync();
        }

        // GET: api/PupilsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Pupil>> GetPupil(int id)
        {
            var pupil = await _context.Pupils
                .FirstOrDefaultAsync(m => m.Id == id);

            if (pupil == null)
            {
                return NotFound();
            }

            return pupil;
        }

        // PUT: api/PupilsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPupil(int id, Pupil pupil)
        {
            if (id != pupil.Id)
            {
                return BadRequest();
            }

            if (pupil.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { message = "Дата народження не може бути датою в майбутньому" });
            }
            if (pupil.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-18)) || pupil.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-5)))
            {
                return BadRequest(new { message = "Учень не може бути молодшим 5 років або старшим за 18" });
            }

            _context.Entry(pupil).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PupilExists(id))
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

        // POST: api/PupilsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Pupil>> PostPupil(Pupil pupil)
        {
            if (pupil.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { message = "Дата народження не може бути датою в майбутньому" });
            }
            if (pupil.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-18)) || pupil.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-5)))
            {
                return BadRequest(new { message = "Учень не може бути молодшим 5 років або старшим за 18" });
            }

            _context.Pupils.Add(pupil);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPupil", new { id = pupil.Id }, pupil);
        }

        // DELETE: api/PupilsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePupil(int id)
        {
            var pupil = await _context.Pupils.FindAsync(id);
            if (pupil == null)
            {
                return NotFound();
            }

            // Видалення записів з PupilsEvents
            var relatedPupilsEvents = _context.PupilsEvents.Where(pe => pe.PupilsId == id);
            _context.PupilsEvents.RemoveRange(relatedPupilsEvents);

            _context.Pupils.Remove(pupil);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Учень успішно видалений" });
        }

        private bool PupilExists(int id)
        {
            return _context.Pupils.Any(e => e.Id == id);
        }
    }
}

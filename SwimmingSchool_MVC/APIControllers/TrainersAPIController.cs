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
    public class TrainersAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public TrainersAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/TrainersAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Trainer>>> GetTrainers()
        {
            return await _context.Trainers.ToListAsync();
        }

        // GET: api/TrainersAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Trainer>> GetTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);

            if (trainer == null)
            {
                return NotFound();
            }

            return trainer;
        }

        // PUT: api/TrainersAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTrainer(int id, Trainer trainer)
        {
            if (id != trainer.Id)
            {
                return BadRequest();
            }

            if (trainer.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { message = "Дата народження не може бути датою в майбутньому" });
            }
            if (trainer.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-75)) || trainer.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
            {
                return BadRequest(new { message = "Вчитель не може бути молодшим 16 років або старшим за 75" });
            }

            _context.Entry(trainer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExists(id))
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

        // POST: api/TrainersAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Trainer>> PostTrainer(Trainer trainer)
        {
            if (trainer.Birthday > DateOnly.FromDateTime(DateTime.Today))
            {
                return BadRequest(new { message = "Дата народження не може бути датою в майбутньому" });
            }
            if (trainer.Birthday < DateOnly.FromDateTime(DateTime.Today.AddYears(-75)) || trainer.Birthday > DateOnly.FromDateTime(DateTime.Today.AddYears(-16)))
            {
                return BadRequest(new { message = "Вчитель не може бути молодшим 16 років або старшим за 75" });
            }

            _context.Trainers.Add(trainer);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTrainer", new { id = trainer.Id }, trainer);
        }

        // DELETE: api/TrainersAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainer(int id)
        {
            var trainer = await _context.Trainers.FindAsync(id);
            if (trainer == null)
            {
                return NotFound();
            }

            if (await _context.Groups.AnyAsync(g => g.TrainerId == trainer.Id))
            {
                return BadRequest(new { message = "Неможливо видалити тренера, оскільки він прив'язаний до групи" });
            }

            _context.Trainers.Remove(trainer);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool TrainerExists(int id)
        {
            return _context.Trainers.Any(e => e.Id == id);
        }
    }
}

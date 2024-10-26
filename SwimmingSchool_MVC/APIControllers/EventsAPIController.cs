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
    public class EventsAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public EventsAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/EventsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            return await _context.Events.ToListAsync();
        }

        // GET: api/EventsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            return @event;
        }

        // PUT: api/EventsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvent(int id, Event @event)
        {
            if (id != @event.Id)
            {
                return BadRequest();
            }

            if (@event.MaxPupilsAmount <= 0)
            {
                return BadRequest(new { message = "Кількість учасників повинна бути більше 0" });
            }
            if (@event.Date < DateTime.Now)
            {
                return BadRequest(new { message = "Дата не може бути в минулому" });
            }

            _context.Entry(@event).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(id))
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

        // POST: api/EventsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Event>> PostEvent(Event @event)
        {
            if (@event.MaxPupilsAmount <= 0)
            {
                return BadRequest(new { message = "Кількість учасників повинна бути більше 0" });
            }
            if (@event.Date < DateTime.Now)
            {
                return BadRequest(new { message = "Дата не може бути в минулому" });
            }

            @event.IsHeld = false;
            _context.Events.Add(@event);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvent", new { id = @event.Id }, @event);
        }

        // DELETE: api/EventsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            var relatedPupilsEvents = _context.PupilsEvents.Where(pe => pe.EventId == id);
            _context.PupilsEvents.RemoveRange(relatedPupilsEvents);

            _context.Events.Remove(@event);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }

        [HttpPost("CheckEvent")]
        public IActionResult CheckEvent()
        {
            var events = _context.Events.Where(e => !e.IsHeld && e.Date <= DateTime.Now);
            foreach (var @event in events)
            {
                @event.IsHeld = true;
            }
            _context.SaveChanges();

            return Ok();
        }
    }
}

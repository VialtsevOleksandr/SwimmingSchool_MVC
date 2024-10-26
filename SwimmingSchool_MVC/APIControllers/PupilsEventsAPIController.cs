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
    public class PupilsEventsAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public PupilsEventsAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/PupilsEventsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PupilsEvent>>> GetPupilsEvents()
        {
            return await _context.PupilsEvents.ToListAsync();
        }

        // GET: api/PupilsEventsAPI/5/10
        [HttpGet("{pupilsId}/{eventId}")]
        public async Task<ActionResult<PupilsEvent>> GetPupilsEvent(int pupilsId, int eventId)
        {
            var pupilsEvent = await _context.PupilsEvents.FindAsync(pupilsId, eventId);

            if (pupilsEvent == null)
            {
                return NotFound();
            }

            return pupilsEvent;
        }

        // PUT: api/PupilsEventsAPI/5/10
        [HttpPut("{pupilsId}/{eventId}")]
        public async Task<IActionResult> PutPupilsEvent(int pupilsId, int eventId, PupilsEvent pupilsEvent)
        {
            if (pupilsId != pupilsEvent.PupilsId || eventId != pupilsEvent.EventId)
            {
                return BadRequest();
            }

            _context.Entry(pupilsEvent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PupilsEventExists(pupilsId, eventId))
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

        // POST: api/PupilsEventsAPI
        [HttpPost]
        public async Task<ActionResult<PupilsEvent>> PostPupilsEvent(PupilsEvent pupilsEvent)
        {
            var evt = await _context.Events.FindAsync(pupilsEvent.EventId);
            if (evt == null)
            {
                return NotFound();
            }

            var currentRegisteredPupilsCount = GetRegisteredPupilsCount(pupilsEvent.EventId);
            if (currentRegisteredPupilsCount >= evt.MaxPupilsAmount)
            {
                return BadRequest("Не можна додати більше учнів, ніж вказано в максимальній кількості учасників.");
            }

            if (_context.PupilsEvents.Any(pe => pe.PupilsId == pupilsEvent.PupilsId && pe.EventId == pupilsEvent.EventId))
            {
                return Conflict("Учень вже зареєстрований на цю подію.");
            }
            pupilsEvent.Info = string.IsNullOrEmpty(pupilsEvent.Info) ? "Зареєстрований" : pupilsEvent.Info;
            _context.PupilsEvents.Add(pupilsEvent);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (PupilsEventExists(pupilsEvent.PupilsId, pupilsEvent.EventId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetPupilsEvent", new { pupilsId = pupilsEvent.PupilsId, eventId = pupilsEvent.EventId }, pupilsEvent);
        }

        // DELETE: api/PupilsEventsAPI/5/10
        [HttpDelete("{pupilsId}/{eventId}")]
        public async Task<IActionResult> DeletePupilsEvent(int pupilsId, int eventId)
        {
            var pupilsEvent = await _context.PupilsEvents.FindAsync(pupilsId, eventId);
            if (pupilsEvent == null)
            {
                return NotFound();
            }

            _context.PupilsEvents.Remove(pupilsEvent);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PupilsEventExists(int pupilsId, int eventId)
        {
            return _context.PupilsEvents.Any(e => e.PupilsId == pupilsId && e.EventId == eventId);
        }
        private int GetRegisteredPupilsCount(int eventId)
        {
            return _context.PupilsEvents.Count(pe => pe.EventId == eventId);
        }
    }
}
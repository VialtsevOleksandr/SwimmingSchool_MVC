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
    public class GroupSchedulesAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupSchedulesAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/GroupSchedulesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupSchedule>>> GetGroupSchedules()
        {
            return await _context.GroupSchedules.ToListAsync();
        }

        // GET: api/GroupSchedulesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupSchedule>> GetGroupSchedule(int id)
        {
            var groupSchedule = await _context.GroupSchedules.FindAsync(id);

            if (groupSchedule == null)
            {
                return NotFound();
            }

            return groupSchedule;
        }

        // PUT: api/GroupSchedulesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupSchedule(int id, GroupSchedule groupSchedule)
        {
            if (id != groupSchedule.Id)
            {
                return BadRequest();
            }

            if (_context.GroupSchedules.Any(g => g.GroupId == groupSchedule.GroupId && g.DayId == groupSchedule.DayId))
            {
                return BadRequest(new { message = "Група не може мати більше тренувань у цей день" });
            }

            if (groupSchedule.TrainingTime <= 0)
            {
                return BadRequest(new { message = "Тривалість тренування повинна бути більше 0" });
            }

            _context.Entry(groupSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupScheduleExists(id))
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

        // POST: api/GroupSchedulesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroupSchedule>> PostGroupSchedule(GroupSchedule groupSchedule)
        {
            if (_context.GroupSchedules.Any(g => g.GroupId == groupSchedule.GroupId && g.DayId == groupSchedule.DayId))
            {
                return BadRequest(new { message = "Група не може мати більше тренувань у цей день" });
            }

            if (groupSchedule.TrainingTime <= 0)
            {
                return BadRequest(new { message = "Тривалість тренування повинна бути більше 0" });
            }

            _context.GroupSchedules.Add(groupSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupSchedule", new { id = groupSchedule.Id }, groupSchedule);
        }

        // DELETE: api/GroupSchedulesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupSchedule(int id)
        {
            var groupSchedule = await _context.GroupSchedules.FindAsync(id);
            if (groupSchedule == null)
            {
                return NotFound();
            }

            _context.GroupSchedules.Remove(groupSchedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupScheduleExists(int id)
        {
            return _context.GroupSchedules.Any(e => e.Id == id);
        }
    }
}

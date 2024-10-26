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
    public class GroupsAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupsAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/GroupsAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Group>>> GetGroups()
        {
            return await _context.Groups
                .Include(g => g.Pupils)
                .Include(g => g.GroupSchedules)
                .ToListAsync();
        }

        // GET: api/GroupsAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Group>> GetGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Pupils)
                .Include(g => g.GroupSchedules)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            return group;
        }

        // PUT: api/GroupsAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroup(int id, Group group)
        {
            if (id != group.Id)
            {
                return BadRequest();
            }

            if (_context.Groups.Any(g => g.GroupName == group.GroupName && g.TrainerId == group.TrainerId))
            {
                return BadRequest(new { message = "Група з таким ім'ям та тренером вже існує" });
            }

            _context.Entry(group).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupExists(id))
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

        // POST: api/GroupsAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Group>> PostGroup(Group group)
        {
            if (_context.Groups.Any(g => g.GroupName == group.GroupName && g.TrainerId == group.TrainerId))
            {
                return BadRequest(new { message = "Група з таким ім'ям та тренером вже існує" });
            }

            _context.Groups.Add(group);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroup", new { id = group.Id }, group);
        }

        // DELETE: api/GroupsAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroup(int id)
        {
            var group = await _context.Groups
                .Include(g => g.Pupils)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (group == null)
            {
                return NotFound();
            }

            if (await _context.Pupils.AnyAsync(p => p.GroupId == group.Id))
            {
                return BadRequest(new { message = "Неможливо видалити групу, оскільки в ній є учні" });
            }

            _context.Groups.Remove(group);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool GroupExists(int id)
        {
            return _context.Groups.Any(e => e.Id == id);
        }
    }
}

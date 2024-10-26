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
    public class GroupTypesAPIController : ControllerBase
    {
        private readonly DbSwimmingSchoolContext _context;

        public GroupTypesAPIController(DbSwimmingSchoolContext context)
        {
            _context = context;
        }

        // GET: api/GroupTypesAPI
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GroupType>>> GetGroupTypes()
        {
            return await _context.GroupTypes.ToListAsync();
        }

        // GET: api/GroupTypesAPI/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupType>> GetGroupType(int id)
        {
            var groupType = await _context.GroupTypes.FindAsync(id);

            if (groupType == null)
            {
                return NotFound();
            }

            return groupType;
        }

        // PUT: api/GroupTypesAPI/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupType(int id, GroupType groupType)
        {
            if (id != groupType.Id)
            {
                return BadRequest();
            }
            if (_context.GroupTypes.Any(g => g.Name == groupType.Name))
            {
                return BadRequest(new { message = "Такий тип групи вже існує" });
            }
            _context.Entry(groupType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupTypeExists(id))
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

        // POST: api/GroupTypesAPI
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<GroupType>> PostGroupType(GroupType groupType)
        {
            if (_context.GroupTypes.Any(g => g.Name == groupType.Name))
            {
                return BadRequest(new { message = "Такий тип групи вже існує" });
            }
            _context.GroupTypes.Add(groupType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetGroupType", new { id = groupType.Id }, groupType);
        }

        // DELETE: api/GroupTypesAPI/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupType(int id)
        {
            var groupType = await _context.GroupTypes.FindAsync(id);
            if (groupType == null)
            {
                return NotFound();
            }
            if (await _context.Groups.AnyAsync(g => g.GroupTypeId == groupType.Id))
            {
                return BadRequest(new { message = "Не можна видалити тип групи, який пов'язаний з групами" });
            }
            _context.GroupTypes.Remove(groupType);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Тип групи успішно видалено" });
        }

        private bool GroupTypeExists(int id)
        {
            return _context.GroupTypes.Any(e => e.Id == id);
        }
    }
}

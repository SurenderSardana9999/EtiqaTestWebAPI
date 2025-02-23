using EtiqaTestAPI.DBContext;
using EtiqaTestAPI.Entity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EtiqaTestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FreelancersController : ControllerBase
    {
        private readonly AppDbContext _context;
        public FreelancersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Freelancer>>> GetFreelancers()
        {
            return await _context.Freelancers.Include(f => f.Skillsets).Include(f => f.Hobbies).Where(f => !f.IsArchived).ToListAsync();
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Freelancer>>> SearchFreelancers(string query)
        {
            return await _context.Freelancers
                .Where(f => f.Username.Contains(query) || f.Email.Contains(query))
                .Include(f => f.Skillsets).Include(f => f.Hobbies)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<ActionResult<Freelancer>> RegisterFreelancer([FromBody] Freelancer freelancer)
        {
            if (freelancer == null)
            {
                return BadRequest("Invalid data.");
            }
            _context.Freelancers.Add(freelancer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetFreelancers), new { id = freelancer.Id }, freelancer);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFreelancer(int id, Freelancer freelancer)
        {
            if (id != freelancer.Id) return BadRequest();
            _context.Entry(freelancer).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFreelancer(int id)
        {
            var freelancer = await _context.Freelancers.FindAsync(id);
            if (freelancer == null) return NotFound();
            _context.Freelancers.Remove(freelancer);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("archive/{id}")]
        public async Task<IActionResult> ArchiveFreelancer(int id)
        {
            var freelancer = await _context.Freelancers.FindAsync(id);
            if (freelancer == null) return NotFound();
            freelancer.IsArchived = true;
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}

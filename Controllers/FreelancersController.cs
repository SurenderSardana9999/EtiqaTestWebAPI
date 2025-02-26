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
        [ResponseCache(Duration = 10, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<ActionResult<IEnumerable<Freelancer>>> GetFreelancers()
        {
            var freelancers = await _context.Freelancers.AsNoTracking()
        .Include(f => f.SkillMappings)
        .Select(f => new
        {
            f.Id,
            f.Username,
            f.Email,
            f.PhoneNumber,
            f.IsArchived,
            Skills = f.SkillMappings
                .Where(sm => sm.SkllsId != null)
                .Select(sm => _context.Skills.FirstOrDefault(s => s.Id == sm.SkllsId).Name)
                .ToList(),
            Hobbies = f.SkillMappings
                .Where(sm => sm.HobbiesId != null)
                .Select(sm => _context.Hobbies.FirstOrDefault(h => h.Id == sm.HobbiesId).Name)
                .ToList()
        })
        .ToListAsync();

            return Ok(freelancers);
        }


        [HttpGet("search")] 
        public async Task<ActionResult<IEnumerable<Freelancer>>> SearchFreelancers(string query) =>
        await _context.Freelancers.AsNoTracking().Where(f => f.Username.Contains(query) || f.Email.Contains(query)).ToListAsync();


        [HttpGet("paged")]
        public async Task<ActionResult<IEnumerable<Freelancer>>> GetFreelancersPaged([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var totalRecords = await _context.Freelancers.CountAsync();
            var freelancers = await _context.Freelancers.AsNoTracking()
                                            .Include(f => f.SkillMappings)
        .Select(f => new
        {
            f.Id,
            f.Username,
            f.Email,
            f.PhoneNumber,
            f.IsArchived,
            Skills = f.SkillMappings
                .Where(sm => sm.SkllsId != null)
                .Select(sm => _context.Skills.FirstOrDefault(s => s.Id == sm.SkllsId).Name)
                .ToList(),
            Hobbies = f.SkillMappings
                .Where(sm => sm.HobbiesId != null)
                .Select(sm => _context.Hobbies.FirstOrDefault(h => h.Id == sm.HobbiesId).Name)
                .ToList()
        })
                                            .Skip((page - 1) * pageSize)
                                            .Take(pageSize)
                                            .ToListAsync();

            return Ok(new
            {
                TotalRecords = totalRecords,
                Page = page,
                PageSize = pageSize,
                Data = freelancers
            });
        }

        [HttpGet("hobbies")]
        public async Task<ActionResult<IEnumerable<Hobby>>> getHobbies() =>
        await _context.Hobbies.ToListAsync();


        [HttpGet("skills")]
        public async Task<ActionResult<IEnumerable<Skill>>> getSkills() =>
        await _context.Skills.ToListAsync();


        [HttpPost]
        public async Task<ActionResult<Freelancer>> RegisterFreelancer(Freelancer freelancer)
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.NewFolder;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly MovieDbContext _context;
        public PersonsController(MovieDbContext  context)
        {
            _context = context;
                
        }
        [HttpPost]
        public async Task<IActionResult> CreateActor(Person person)
        {
            // Check if a person with the same ID already exists
            bool exists = await _context.Persons.AnyAsync(x => x.Id == person.Id);

            if (exists)
            {
                return BadRequest(new
                {
                    Status = false,
                    Message = "Actor already exists."
                });
            }

            // Add new actor
            await _context.Persons.AddAsync(person);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Status = true,
                Message = "Actor created successfully.",
                Data = person
            });
        }

    }
}

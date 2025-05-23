using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieAPI.Data;
using MovieAPI.Models;
using MovieAPI.NewFolder;

namespace MovieAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonsController : ControllerBase
    {
        private readonly MovieDbContext _context;

        private readonly IMapper _mapper;


        public PersonsController(MovieDbContext  context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
                
        }
        [HttpGet]
        public async Task<IActionResult> Get(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var actorCount = await _context.Persons.CountAsync();
                var actorList = _mapper.Map<List<ActorsViewModel>> (await _context.Persons.Skip(pageIndex * pageSize).Take(pageSize)
                    .ToListAsync());
                response.Status = true;
                response.Message = "Success";
                response.Data = new { Person =  actorList, Count = actorCount };
                return Ok(response);
            }
            catch (Exception)
            {

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetActorById(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var actor = await _context.Persons.FirstOrDefaultAsync(x => x.Id == id);
                if (actor == null)
                {
                    response.Status = false;
                    response.Message = "Record not exist";
                    return BadRequest(response);

                }
                var actorData = new ActorDetailsModel
                {
                    Id=actor.Id,
                    Name=actor.Name,
                    DateOfBirth=actor.DateOfBirth,
                    Movies = await _context.Movies
                    .Where(x => x.Actors.Contains(actor))
                    .Select(x => x.Title).ToArrayAsync(),
                };
                response.Status = true;
                response.Message = "Success";
                response.Data = actorData;
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.Status = false;
                response.Message = ex.Message;
                return BadRequest(response);
            }
        }



        [HttpPost]
        public async Task<IActionResult> CreateActor(ActorsViewModel person)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var postedModel = new Person()
                    {
                        Name = person.Name,
                        DateOfBirth = person.DateOfBirth
                    };
                    await _context.Persons.AddAsync(postedModel);
                   await _context.SaveChangesAsync();
                    person.Id = postedModel.Id;
                    response.Status = true;
                    response.Message = "Created Successfully";
                    response.Data = person;

                    return Ok(response);

                }
                else
                {
                    response.Status = false;
                    response.Message = "Validation failed";
                    response.Data = ModelState;
                    return BadRequest(response);
                }

            }
            catch (Exception ex)
            {

                response.Status= false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }

            //// Check if a person with the same ID already exists
            //bool exists = await _context.Persons.AnyAsync(x => x.Id == person.Id);

            //if (exists)
            //{
            //    return BadRequest(new
            //    {
            //        Status = false,
            //        Message = "Actor already exists."
            //    });
            //}
            //var movies = await _context.Movies.Where(m => person.MoviesId.Contains(m.Id)).ToListAsync();

            //var actors = new Person
            //{
            //    Id = person.Id,
            //    Name = person.Name,
            //    DateOfBirth = person.DateOfBirth,
            //    Movies = movies

            //};

            //// Add new actor
            //await _context.Persons.AddAsync(actors);
            //await _context.SaveChangesAsync();

            //return Ok(new
            //{
            //    Status = true,
            //    Message = "Actor created successfully.",
            //    Data = person
            //});



        }

    }
}

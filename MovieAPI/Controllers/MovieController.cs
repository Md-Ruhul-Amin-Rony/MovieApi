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
    public class MovieController : ControllerBase
    {
       
        private readonly MovieDbContext _context;
        public MovieController(MovieDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMovies(int pageIndex = 0, int pageSize = 10)
        {
            BaseResponseModel response = new BaseResponseModel();

            try
            {
                var movieCount = await _context.Movies.CountAsync();
                var movieList = await _context.Movies.Include(a => a.Actors)
            .Skip(pageIndex * pageSize)
            .Take(pageSize)
            .ToListAsync();
                response.Status = true;
                response.Message = "Success";
                response.Data = new {Movies = movieList, Count = movieCount};
                return Ok(response);
            }
            catch (Exception ex)
            {

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
            
        }

        [HttpGet("{id}")]
        public async Task<IActionResult>GetMovie(int id)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                var movie = await _context.Movies.Include(a => a.Actors).FirstOrDefaultAsync(m => m.Id == id);
                if (movie is null)
                {
                    response.Status = false;
                    response.Message = "No Movie found";

                }
                response.Status = true;
                response.Message = "Success";
                response.Data = movie;
                return Ok(response);
            }
            catch (Exception)
            {

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }
           
        }
        [HttpPost]
        public async Task<IActionResult> CreateMovie(CreateMovieViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    var actors = await _context.Persons.Where(a => model.Actors.Contains(a.Id)).ToListAsync();

                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actors assigned";
                        return BadRequest(response);
                    }
                    var postedModel = new Movie()
                    {
                        Title = model.Title,
                        Language = model.Language,
                        ReleaseDate = model.ReleaseDate,
                        CoverImage = model.CoverImage,
                        Description = model.Description,
                        Actors = actors,

                    };
                    await _context.Movies.AddAsync(postedModel);
                   await _context.SaveChangesAsync();
                    response.Status = true;
                    response.Message = "Movie created successfully";
                    response.Data = postedModel;
                    return Ok(response);
                   

                }else
                response.Status = false;
                response.Message = "Invalid data";
                response.Data = ModelState;
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }

        }
        [HttpPut]
        public async Task<IActionResult> EditMovie(CreateMovieViewModel model)
        {
            BaseResponseModel response = new BaseResponseModel();
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.Id <= 0)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie record";
                        return BadRequest(response);

                    }
                    var actors = await _context.Persons.Where(a => model.Actors.Contains(a.Id)).ToListAsync();

                    if (actors.Count != model.Actors.Count)
                    {
                        response.Status = false;
                        response.Message = "Invalid Actors assigned";
                        return BadRequest(response);
                    }

                    var movieDetails = _context.Movies.Include(a => a.Actors).Where(a => a.Id == model.Id).FirstOrDefault();

                    if (movieDetails == null)
                    {
                        response.Status = false;
                        response.Message = "Invalid Movie record";
                        return BadRequest(response);
                    }
                    movieDetails.CoverImage = model.CoverImage;
                    movieDetails.Description = model.Description;
                    movieDetails.Language = model.Language;
                    movieDetails.ReleaseDate = model.ReleaseDate;
                    movieDetails.Title = model.Title;

                    // Find the removed Actors
                    var removedActors = movieDetails.Actors.Where(a => !model.Actors.Contains(a.Id)).ToList();
                    foreach (var actor in removedActors)
                    {
                        movieDetails.Actors.Remove(actor);

                    }
                    var addedActors = actors.Except(movieDetails.Actors).ToList();
                    foreach (var actor in addedActors)
                    {
                        movieDetails.Actors.Add(actor);

                    }

                    await _context.SaveChangesAsync();

                    response.Status = true;
                    response.Message = "Movie Edited successfully";
                    response.Data = movieDetails;
                    return Ok(response);


                }
                else
                    response.Status = false;
                response.Message = "Invalid data";
                response.Data = ModelState;
                return BadRequest(response);
            }
            catch (Exception ex)
            {

                response.Status = false;
                response.Message = "Something went wrong";
                return BadRequest(response);
            }

        }
    }
}


using DotNetCrudWebApi.Data;
using DotNetCrudWebApi.Movies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCrudWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MoviesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public MoviesController(AppDbContext AppDbContext)
        {
            _appDbContext = AppDbContext;
        }

        // Get : api/Movies
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovieModel>>> GetMovies()
        {
            if (_appDbContext.Movies == null)
            {
                return NotFound();
            }
            return await _appDbContext.Movies.ToListAsync();
        }

        // Get : api/Movies/2
        [HttpGet("{id}")]
        public async Task<ActionResult<MovieModel>> GetMovie(int id)
        {
            if (_appDbContext.Movies is null)
            {
                return NotFound();
            }
            var movie = await _appDbContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound();
            }
            return movie;
        }

        // Post : api/Movies
        [HttpPost]
        public async Task<ActionResult<MovieModel>> PostMovie(MovieModel movie)
        {
            _appDbContext.Movies.Add(movie);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetMovie), new { id = movie.Id }, movie);
        }

        // Put : api/Movies/2
        [HttpPut("{id}")]
        public async Task<ActionResult> PutMovie(int id, MovieModel movie)
        {
            // Проверяем, существует ли фильм с данным ID
            if (id != movie.Id)
            {
                return BadRequest("ID фильма не совпадает с переданным ID.");
            }

            // Ищем существующий объект
            var existingMovie = await _appDbContext.Movies.FindAsync(id);
            if (existingMovie == null)
            {
                return NotFound("Фильм не найден.");
            }
            // Обновляем состояние объекта
            existingMovie.Title = movie.Title;
            existingMovie.Genre = movie.Genre; // Убедитесь, что вы обновляете нужные поля
            existingMovie.ReleaseDate = movie.ReleaseDate;

            // Обновляем в базе данных
            try
            {
                _appDbContext.Entry(existingMovie).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound("Фильм не найден.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent(); // Возвращаем 204 No Content
        }
        private bool MovieExists(int id)
        {
            return _appDbContext.Movies.Any(movie => movie.Id == id);
        }

        // Delete : api/Movies/2
        [HttpDelete("{id}")]
        public async Task<ActionResult<MovieModel>> DeleteMovie(int id)
        {
            if (_appDbContext.Movies is null)
            {
                return NotFound();
            }
            var movie = await _appDbContext.Movies.FindAsync(id);
            if (movie is null)
            {
                return NotFound();
            }
            _appDbContext.Movies.Remove(movie);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}

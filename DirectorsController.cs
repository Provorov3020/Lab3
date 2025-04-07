using DotNetCrudWebApi.Data;
using DotNetCrudWebApi.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCrudWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DirectorsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public DirectorsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Get : api/Directors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DirectorModel>>> GetDirectors()
        {
            if (_appDbContext.Directors == null)
            {
                return NotFound();
            }
            return await _appDbContext.Directors.ToListAsync();
        }

        // Get : api/Directors/2
        [HttpGet("{id}")]
        public async Task<ActionResult<DirectorModel>> GetDirector(int id)
        {
            if (_appDbContext.Directors is null)
            {
                return NotFound();
            }
            var director = await _appDbContext.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            return director;
        }

        // Post : api/Directors
        [HttpPost]
        public async Task<ActionResult<DirectorModel>> PostDirector(DirectorModel director)
        {
            _appDbContext.Directors.Add(director);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetDirector), new { id = director.Id }, director);
        }

        // Put : api/Directors/2
        [HttpPut("{id}")]
        public async Task<ActionResult> PutDirector(int id, DirectorModel director)
        {
            if (id != director.Id)
            {
                return BadRequest("ID режиссера не совпадает с переданным ID.");
            }

            var existingDirector = await _appDbContext.Directors.FindAsync(id);
            if (existingDirector == null)
            {
                return NotFound("Режиссер не найден.");
            }

            existingDirector.Name = director.Name;
            existingDirector.LastName = director.LastName; // Убедитесь, что вы обновляете нужные поля
            existingDirector.DateofBirth = director.DateofBirth;

            try
            {
                _appDbContext.Entry(existingDirector).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DirectorExists(id))
                {
                    return NotFound("Режиссер не найден.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent(); // Возвращаем 204 No Content
        }

        private bool DirectorExists(int id)
        {
            return _appDbContext.Directors.Any(director => director.Id == id);
        }

        // Delete : api/Directors/2
        [HttpDelete("{id}")]
        public async Task<ActionResult<DirectorModel>> DeleteDirector(int id)
        {
            if (_appDbContext.Directors == null)
            {
                return NotFound();
            }
            var director = await _appDbContext.Directors.FindAsync(id);
            if (director == null)
            {
                return NotFound();
            }
            _appDbContext.Directors.Remove(director);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }

}


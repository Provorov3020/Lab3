using DotNetCrudWebApi.Data;
using DotNetCrudWebApi.Movies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCrudWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorsController: ControllerBase
    {
        private readonly AppDbContext _appDbContext;

        public ActorsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        // Get : api/Actors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActorModel>>> GetActors()
        {
            if (_appDbContext.Actors == null)
            {
                return NotFound();
            }
            return await _appDbContext.Actors.ToListAsync();
        }

        // Get : api/Actors/2
        [HttpGet("{id}")]
        public async Task<ActionResult<ActorModel>> GetActor(int id)
        {
            if (_appDbContext.Actors is null)
            {
                return NotFound();
            }
            var actor = await _appDbContext.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            return actor;
        }

        // Post : api/Actors
        [HttpPost]
        public async Task<ActionResult<ActorModel>> PostActor(ActorModel actor)
        {
            _appDbContext.Actors.Add(actor);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetActor), new { id = actor.Id }, actor);
        }

        // Put : api/Actors/2
        [HttpPut("{id}")]
        public async Task<ActionResult> PutActor(int id, ActorModel actor)
        {
            if (id != actor.Id)
            {
                return BadRequest("ID актера не совпадает с переданным ID.");
            }

            var existingActor = await _appDbContext.Actors.FindAsync(id);
            if (existingActor == null)
            {
                return NotFound("Актер не найден.");
            }

            existingActor.Name = actor.Name;
            existingActor.LastName = actor.LastName; // Убедитесь, что вы обновляете нужные поля
           

            try
            {
                _appDbContext.Entry(existingActor).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActorExists(id))
                {
                    return NotFound("Актер не найден.");
                }
                else
                {
                    throw;
                }
            }
            return NoContent(); // Возвращаем 204 No Content
        }

        private bool ActorExists(int id)
        {
            return _appDbContext.Actors.Any(actor => actor.Id == id);
        }

        // Delete : api/Actors/2
        [HttpDelete("{id}")]
        public async Task<ActionResult<ActorModel>> DeleteActor(int id)
        {
            if (_appDbContext.Actors == null)
            {
                return NotFound();
            }
            var actor = await _appDbContext.Actors.FindAsync(id);
            if (actor == null)
            {
                return NotFound();
            }
            _appDbContext.Actors.Remove(actor);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}


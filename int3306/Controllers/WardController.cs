using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Route("/wards")]
    public class WardController : Controller
    {
        private readonly DataDbContext dbContext;
        public WardController(DataDbContext dbContext) => this.dbContext = dbContext; 
        
        /// <summary>
        /// Create a ward.
        /// </summary>
        /// <param name="ward">The details relevant to create a new ward.</param>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Ward>> Create([FromBody] Ward ward)
        {
            var toInsert = new Ward { WardName = ward.WardName };
            var result = await dbContext.Wards.AddAsync(toInsert);
            await dbContext.SaveChangesAsync();
            return result.Entity;
        }

        /// <summary>
        /// Get a ward from its ID.
        /// </summary>
        /// <param name="id">Ward ID to get</param>
        /// <returns>The relevant ward, or a 404 Not Found if there's none.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Ward>> Get(int id)
        {
            var result = await dbContext.Wards.FirstOrDefaultAsync(w => w.WardId == id);
            return result == null ? NotFound() : result;
        }
        
        /// <summary>
        /// List all wards.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<Ward[]>> List()
        {
            return await dbContext.Wards.ToArrayAsync();
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await dbContext.Wards.FirstOrDefaultAsync(w => w.WardId == id);
            if (result == null) return NotFound();
            dbContext.Remove(result);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
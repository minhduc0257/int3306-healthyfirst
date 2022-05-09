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
            if (!ModelState.IsValid) return BadRequest();

            if (!await dbContext.Districts.AnyAsync(d => d.DistrictId == ward.DistrictId))
            {
                return NotFound("non-existent district id");
            }

            var toInsert = new Ward { WardName = ward.WardName, DistrictId = ward.DistrictId };
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

        /// <summary>
        /// Update ward with new data, preserving ID.
        /// </summary>
        /// <param name="id">Ward ID</param>
        /// <param name="ward">Details to replace.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Ward ward)
        {
            if (ModelState.IsValid)
            {
                return BadRequest();
            }
            
            var wardInDb = await dbContext.Wards.FirstOrDefaultAsync(s => s.WardId == id);
            if (wardInDb == null)
            {
                return NotFound();
            }
            
            dbContext.Entry(wardInDb).State = EntityState.Detached;
            ward.WardId = wardInDb.WardId;
            dbContext.Wards.Attach(ward);
            dbContext.Entry(ward).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await dbContext.Wards.FirstOrDefaultAsync(w => w.WardId == id);
            if (result == null) return NotFound();
            if (await dbContext.Shops.AnyAsync(s => s.Ward == result.WardId))
            {
                return BadRequest("at least a shop exists with this ward!");
            }
            
            dbContext.Remove(result);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
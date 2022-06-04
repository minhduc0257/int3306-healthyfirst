using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [RequirePrivileged]
    [ApiController]
    [Route("/wards")]
    public class WardController : ExtendedController
    {
        public WardController(DataDbContext dbContext) : base(dbContext) {} 
        
        /// <summary>
        /// Create a ward.
        /// </summary>
        /// <param name="ward">The details relevant to create a new ward.</param>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Ward>> Create([FromBody] Ward ward)
        {
            if (!ModelState.IsValid) return BadRequest();

            if (!await DBContext.Districts.AnyAsync(d => d.DistrictId == ward.DistrictId))
            {
                return NotFound("non-existent district id");
            }

            var toInsert = new Ward { WardName = ward.WardName, DistrictId = ward.DistrictId };
            var result = await DBContext.Wards.AddAsync(toInsert);
            await DBContext.SaveChangesAsync();
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
            var result = await DBContext.Wards.FirstOrDefaultAsync(w => w.WardId == id);
            return result == null ? NotFound() : result;
        }
        
        /// <summary>
        /// List all wards.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<Ward[]>> List()
        {
            return await DBContext.Wards.ToArrayAsync();
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
            
            var wardInDb = await DBContext.Wards.FirstOrDefaultAsync(s => s.WardId == id);
            if (wardInDb == null)
            {
                return NotFound();
            }
            
            DBContext.Entry(wardInDb).State = EntityState.Detached;
            ward.WardId = wardInDb.WardId;
            DBContext.Wards.Attach(ward);
            DBContext.Entry(ward).State = EntityState.Modified;

            await DBContext.SaveChangesAsync();
            return Ok();
        }
        
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await DBContext.Wards.FirstOrDefaultAsync(w => w.WardId == id);
            if (result == null) return NotFound();
            if (await DBContext.Shops.AnyAsync(s => s.Ward == result.WardId))
            {
                return BadRequest("at least a shop exists with this ward!");
            }
            
            DBContext.Remove(result);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
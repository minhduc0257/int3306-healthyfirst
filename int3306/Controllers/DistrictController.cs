using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [RequirePrivileged]
    [ApiController]
    [Route("/districts")]
    public class DistrictController : ExtendedController
    {
        public DistrictController(DataDbContext dbContext) : base(dbContext) {} 
        
        /// <summary>
        /// Create a District.
        /// </summary>
        /// <param name="district">The details relevant to create a new District.</param>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<District>> Create([FromBody] District district)
        {
            if (!ModelState.IsValid) return BadRequest();
            
            var toInsert = new District { DistrictName = district.DistrictName };
            var result = await DBContext.Districts.AddAsync(toInsert);
            await DBContext.SaveChangesAsync();
            return result.Entity;
        }

        /// <summary>
        /// Get a District from its ID.
        /// </summary>
        /// <param name="id">District ID to get</param>
        /// <returns>The relevant District, or a 404 Not Found if there's none.</returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<District>> Get(int id)
        {
            var result = await DBContext.Districts
                .Include(d => d.Wards.Where(w => w.DistrictId == id))    
                .FirstOrDefaultAsync(w => w.DistrictId == id);
            return result == null ? NotFound() : result;
        }
        
        /// <summary>
        /// List all Districts.
        /// </summary>
        [HttpGet]
        [Route("")]
        public async Task<ActionResult<District[]>> List()
        {
            return await DBContext.Districts.ToArrayAsync();
        }
        
        [HttpDelete]
        [Route("{districtId:int}")]
        public async Task<IActionResult> Delete(int districtId)
        {
            var result = await DBContext.Districts.FirstOrDefaultAsync(w => w.DistrictId == districtId);
            if (result == null) return NotFound();
            
            if (await DBContext.Wards.AnyAsync(w => w.DistrictId == result.DistrictId)
                || await DBContext.Shops.AnyAsync(w => w.District == result.DistrictId))
            {
                return BadRequest("exist at least one ward or shop with this district id");
            }

            DBContext.Remove(result);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
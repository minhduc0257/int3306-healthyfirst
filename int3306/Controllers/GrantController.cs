using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [ApiController]
    [Route("grants")]
    [RequirePrivileged]
    public class GrantController : ExtendedController
    {
        public GrantController(DataDbContext dbContext) : base(dbContext) {}

        /// <summary>
        /// List grants.
        /// </summary>
        /// <param name="userId">If passed, filter by this user ID.</param>
        /// <param name="districtId">If passed, filter by this district ID.</param>
        /// <param name="wardId">If passed, filter by this ward ID.</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Grant[]>> List(
            [FromQuery(Name = "user")] int? userId = null,
            [FromQuery(Name = "district")] int? districtId = null,
            [FromQuery(Name = "ward")] int? wardId = null
        )
        {
            IQueryable<Grant> query = DBContext.Grants;
            if (userId.HasValue)
                query = query.Where(g => g.UserId == userId.Value);
            
            if (districtId.HasValue)
                query = query.Where(g => g.DistrictId == districtId.Value);
            
            if (wardId.HasValue)
                query = query.Where(g => g.WardId == wardId.Value);

            return await query.ToArrayAsync();
        }
        
        /// <summary>
        /// Create a grant for an user. Must pass either a district or ward ID (but not both at the same time).
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Grant>> Create([FromBody] Grant grant)
        {
            var newGrant = new Grant
            {
                UserId = grant.UserId,
                DistrictId = grant.DistrictId,
                WardId = grant.WardId
            };
            if (grant.DistrictId == null && grant.WardId == null)
                return BadRequest("you must grant either a ward or a district only");
            
            if (grant.DistrictId != null && grant.WardId != null)
                return BadRequest("you must grant either a ward or a district only");

            var res = DBContext.Add(newGrant);
            await DBContext.SaveChangesAsync();
            return res.Entity;
        }

        /// <summary>
        /// Delete a grant.
        /// </summary>
        /// <param name="id">ID of the grant to delete.</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var grant = await DBContext.Grants.FirstOrDefaultAsync(g => g.Id == id);
            if (grant is null)
            {
                return NotFound();
            }

            DBContext.Remove(grant);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
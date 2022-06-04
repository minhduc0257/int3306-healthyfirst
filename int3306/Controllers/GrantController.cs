using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [ApiController]
    [Route("grants")]
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
    }
}
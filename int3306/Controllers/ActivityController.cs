using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [ApiController]
    [Route("activities")]
    public class ActivityController : ExtendedController
    {
        public ActivityController(DataDbContext dbContext) : base(dbContext) {}

        /// <summary>
        /// List activities
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Activity[]>> List(
            [FromQuery(Name = "shopId")] int? shopId = null    
        )
        {
            IQueryable<Activity> q = DBContext.Activity;
            if (shopId is not null) q = q.Where(a => a.ShopId == shopId.Value);

            return await q.ToArrayAsync();
        }

        /// <summary>
        /// Create an activity.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Activity>> Create([FromBody] Activity activity)
        {
            if (activity.StartTime > activity.EndTime)
            {
                return BadRequest("activity start time later than end time");
            }

            if (!await DBContext.Plans.AnyAsync(p => p.PlanId == activity.PlanId))
            {
                return BadRequest($"non-existent plan id {activity.PlanId}");
            }
            
            if (!await DBContext.Shops.AnyAsync(s => s.Id == activity.ShopId))
            {
                return BadRequest($"non-existent shop id {activity.ShopId}");
            }
            
            if (!Enum.IsDefined(activity.CurrentStep)) return BadRequest("invalid step");

            var a = new Activity
            {
                PlanId = activity.PlanId,
                ShopId = activity.ShopId,
                Result = activity.Result,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                CurrentStep = activity.CurrentStep
            };

            var r = await DBContext.Activity.AddAsync(a);
            await DBContext.SaveChangesAsync();
            return r.Entity;
        }
        
        /// <summary>
        /// Update an activity (use full object)
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Activity>> Replace(int id, [FromBody] Activity activity)
        {
            if (activity.StartTime > activity.EndTime)
            {
                return BadRequest("activity start time later than end time");
            }

            if (!await DBContext.Plans.AnyAsync(p => p.PlanId == activity.PlanId))
            {
                return BadRequest($"non-existent plan id {activity.PlanId}");
            }
            
            if (!await DBContext.Shops.AnyAsync(s => s.Id == activity.ShopId))
            {
                return BadRequest($"non-existent shop id {activity.ShopId}");
            }
            
            if (!Enum.IsDefined(activity.CurrentStep)) return BadRequest("invalid step");

            var activityInDb = await DBContext.Activity.FirstOrDefaultAsync(a => a.Id == id);
            if (activityInDb is null)
            {
                return NotFound($"no activity with id {id}");
            }
            
            var a = new Activity
            {
                PlanId = activity.PlanId,
                ShopId = activity.ShopId,
                Result = activity.Result,
                StartTime = activity.StartTime,
                EndTime = activity.EndTime,
                CurrentStep = activity.CurrentStep
            };
            
            DBContext.Entry(activityInDb).State = EntityState.Detached;
            a.Id = activityInDb.Id;
            DBContext.Activity.Attach(a);
            DBContext.Entry(a).State = EntityState.Modified;
            await DBContext.SaveChangesAsync();

            return a;
        }
    }
}
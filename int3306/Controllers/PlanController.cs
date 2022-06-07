using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [Authorize]
    [ApiController]
    [Route("plans")]
    [RequirePrivileged]
    public class PlanController : ExtendedController
    {
        public PlanController(DataDbContext dbContext) : base(dbContext) {}

        /// <summary>
        /// Create a plan. Just call it without a body.
        /// </summary>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Plan>> Create()
        {
            var p = new Plan();
            var r = await DBContext.Plans.AddAsync(p);
            await DBContext.SaveChangesAsync();
            return r.Entity;
        }
        
        /// <summary>
        /// List plans.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Plan[]>> List()
        {
            return await DBContext.Plans
                .Include(p => p.Activities)
                .ToArrayAsync();
        }
        
        /// <summary>
        /// Get a single plan.
        /// </summary>
        /// <param name="id">Plan ID</param>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Plan>> Get(int id)
        {
            var res = await DBContext.Plans.Include(p => p.Activities).FirstOrDefaultAsync(p => p.PlanId == id);
            return res != null ? res : NotFound();
        }
        
        /// <summary>
        /// Delete a plan.
        /// </summary>
        /// <param name="id">Plan ID to remove</param>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Plan>> Delete(int id)
        {
            var res = await DBContext.Plans
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (res == null) return NotFound();
            if (res.Activities.Any())
            {
                return BadRequest(
                    $"Plan ID {id} is referenced by activity ID {string.Join(',', res.Activities.Select(a => a.Id))}"
                );
            }

            DBContext.Remove(res);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
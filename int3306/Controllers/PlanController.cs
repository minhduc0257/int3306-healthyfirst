using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Route("plans")]
    public class PlanController : Controller
    {
        private readonly DataDbContext dbContext;
        public PlanController(DataDbContext dbContext) => this.dbContext = dbContext;

        /// <summary>
        /// Create a plan. Just call it without a body.
        /// </summary>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Plan>> Create()
        {
            var p = new Plan();
            var r = await dbContext.Plans.AddAsync(p);
            await dbContext.SaveChangesAsync();
            return r.Entity;
        }
        
        /// <summary>
        /// List plans.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Plan[]>> List()
        {
            return await dbContext.Plans
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
            var res = await dbContext.Plans.FirstOrDefaultAsync(p => p.PlanId == id);
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
            var res = await dbContext.Plans
                .Include(p => p.Activities)
                .FirstOrDefaultAsync(p => p.PlanId == id);

            if (res == null) return NotFound();
            if (res.Activities.Any())
            {
                return BadRequest(
                    $"Plan ID {id} is referenced by activity ID {string.Join(',', res.Activities.Select(a => a.Id))}"
                );
            }

            dbContext.Remove(res);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
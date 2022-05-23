using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace int3306.Controllers
{
    [ApiController]
    [Route("shops")]
    public class ShopController : Controller
    {
        private readonly DataDbContext dbContext;
        public ShopController(DataDbContext dbContext) => this.dbContext = dbContext;

        /// <summary>
        /// Create a shop.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult<Shop>> Create([FromBody] Shop shop)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var dist = await dbContext.Districts
                .Include(d => d.DistrictId)
                .FirstOrDefaultAsync(db => db.DistrictId == shop.District);

            if (dist == null) return NotFound($"district id {shop.District} not found");
            if (dist.Wards.All(w => w.WardId != shop.Ward))
                return NotFound($"ward id {shop.Ward} does not belong to district id {shop.District}");
            
            var newEntity = new Shop
            {
                Name = shop.Name,
                PhoneNumber = shop.PhoneNumber,
                Address = shop.Address,
                Ward = shop.Ward,
                District = shop.District,
                IsProducer = shop.IsProducer,
                IsSeller = shop.IsSeller
            };

            var result = await dbContext.Shops.AddAsync(newEntity);
            await dbContext.SaveChangesAsync();
            return result.Entity;
        }

        
        /// <summary>
        /// List shops.
        /// </summary>
        /// <param name="wardId">Filter by ward ID</param>
        /// <param name="districtId">Filter by district ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<Shop[]>> List(
            int? wardId = null,
            int? districtId = null
        )
        {
            IQueryable<Shop> query = dbContext.Shops;
            if (wardId.HasValue)
                query = query.Where(s => s.Ward == wardId);
            
            if (districtId.HasValue)
                query = query.Where(s => s.District == districtId);
            
            var res = await query
                .OrderBy(s => s.Id)
                .ToArrayAsync();

            foreach (var r in res)
            {
                r.Certificates = await dbContext.Certificates
                    .Where(c => c.ShopId == r.Id)
                    .OrderByDescending(c => c.Timestamp)
                    .Take(1)
                    .ToListAsync();
            }

            return res;
        }

        /// <summary>
        /// Get a shop by its ID.
        /// </summary>
        /// <param name="id">Shop ID to get.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Shop>> Get(int id)
        {
            var shop = await dbContext.Shops
                .Include(
                    s => s.Certificates
                        .Where(s => s.ShopId == id)
                        .OrderByDescending(c => c.Timestamp)
                        .Take(1)
                )
                .FirstOrDefaultAsync(s => s.Id == id);

            if (shop == null)
            {
                return NotFound();
            }

            return Json(shop);
        }

        /// <summary>
        /// Fully replace shop data (send all data of the shop in the body).
        /// </summary>
        /// <param name="id">Shop ID to replace</param>
        /// <param name="shop">Data to replace.</param>
        /// <returns></returns>
        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<Shop>> Update(int id, [FromBody] Shop shop)
        {
            if (ModelState.IsValid)
            {
                return BadRequest();
            }

            var shopInDb = await dbContext.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shopInDb == null)
            {
                return NotFound();
            }

            dbContext.Entry(shopInDb).State = EntityState.Detached;
            shop.Id = shopInDb.Id;
            dbContext.Shops.Attach(shop);
            dbContext.Entry(shop).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return shop;
        }

        /// <summary>
        /// Delete a shop.
        /// </summary>
        /// <param name="id">Shop ID to delete</param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id:int}")]
        public async Task<ActionResult<Shop>> Delete(int id)
        {
            var shop = await dbContext.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null)
            {
                return NotFound();
            }

            dbContext.Remove(shop);
            await dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
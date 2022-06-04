using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Route("shops")]
    public class ShopController : ExtendedController
    {
        public ShopController(DataDbContext dbContext) : base(dbContext) {}

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

            var dist = await DBContext.Districts
                .FirstOrDefaultAsync(db => db.DistrictId == shop.District);
            
            if (dist == null) return NotFound($"district id {shop.District} not found");

            var wards = await DBContext.Wards.Where(w => w.DistrictId == dist.DistrictId).ToListAsync();
            
            if (wards.All(w => w.WardId != shop.Ward))
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

            var result = await DBContext.Shops.AddAsync(newEntity);
            await DBContext.SaveChangesAsync();
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
            IQueryable<Shop> query = DBContext.Shops;
            if (wardId.HasValue)
                query = query.Where(s => s.Ward == wardId);
            
            if (districtId.HasValue)
                query = query.Where(s => s.District == districtId);
            
            var res = await query
                .OrderBy(s => s.Id)
                .ToArrayAsync();

            foreach (var r in res)
            {
                r.Certificates = await DBContext.Certificates
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
            var shop = await DBContext.Shops
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

            var shopInDb = await DBContext.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shopInDb == null)
            {
                return NotFound();
            }

            DBContext.Entry(shopInDb).State = EntityState.Detached;
            shop.Id = shopInDb.Id;
            DBContext.Shops.Attach(shop);
            DBContext.Entry(shop).State = EntityState.Modified;

            await DBContext.SaveChangesAsync();
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
            var shop = await DBContext.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null)
            {
                return NotFound();
            }

            DBContext.Remove(shop);
            await DBContext.SaveChangesAsync();
            return Ok();
        }
    }
}
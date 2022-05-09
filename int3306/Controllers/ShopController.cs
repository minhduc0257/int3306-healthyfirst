using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        /// Get a shop by its ID.
        /// </summary>
        /// <param name="id">Shop ID to get.</param>
        /// <returns></returns>
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Shop>> Get(int id)
        {
            var shop = await dbContext.Shops.FirstOrDefaultAsync(s => s.Id == id);
            if (shop == null)
            {
                return NotFound();
            }

            return shop;
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
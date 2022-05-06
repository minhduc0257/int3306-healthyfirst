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
            dbContext.Shops.Attach(shop);
            dbContext.Entry(shop).State = EntityState.Modified;

            await dbContext.SaveChangesAsync();
            return shop;
        }

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
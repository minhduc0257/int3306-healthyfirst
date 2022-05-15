using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace int3306.Controllers
{
    [ApiController]
    [Route("certificates")]
    public class CertificateController : Controller
    {
        private readonly DataDbContext dbContext;
        public CertificateController(DataDbContext dbContext) => this.dbContext = dbContext;

         

        /// <summary>
        /// List cerficates. Support pagination.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<Certificate[]>> List(
            [FromQuery(Name = "p")] [Range(0, int.MaxValue)] int page = 0,
            [FromQuery(Name = "l")] [Range(1, 50)] int limit = 50,
            [FromQuery(Name = "shop")] [Range(0, int.MaxValue)] int? shop = null    
        )
        {
            IQueryable<Certificate> query = dbContext.Certificates;
            if (shop is not null)
            {
                query = query.Where(c => c.ShopId == shop);
            }
            var result = await query
                .Include(c => c.Shop)
                .OrderByDescending(c => c.Timestamp)
                .Skip(page * limit)
                .Take(limit)
                .ToArrayAsync();
            
            foreach (var res in result)
            {
                if (res.Shop != default)
                    res.Shop.Certificates = null!;
            }
            
            return Json(result);
        }

        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult<Certificate>> Get(int id)
        {
            var cert = await dbContext.Certificates
                .Include(c => c.Shop)
                .FirstOrDefaultAsync(c => c.Id == id);
            return cert == default ? NotFound() : cert;
        }

        [HttpPost]
        public async Task<ActionResult<Certificate>> Create([FromBody] Certificate certificate)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var hasShop = await dbContext.Shops.AnyAsync(s => s.Id == certificate.ShopId);
            if (!hasShop) return NotFound($"shop id {certificate.ShopId} not found");
            if (!Enum.IsDefined(certificate.TransactionType)) return BadRequest("invalid certificate type");
            if (certificate.TransactionType == CertificateType.Grant && certificate.Validity is null)
            {
                return BadRequest("certificate granting must include expiry time");
            }

            var cert = new Certificate
            {
                Timestamp = DateTime.Now,
                ShopId = certificate.ShopId,
                TransactionType = certificate.TransactionType,
                Validity = certificate.Validity
            };
            var entry = await dbContext.AddAsync(cert);
            await dbContext.SaveChangesAsync();
            return entry.Entity;
        }
    }
}
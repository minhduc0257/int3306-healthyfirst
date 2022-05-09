using System;
using System.Threading.Tasks;
using int3306.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace int3306.Tests
{
    public class DistrictControllerTest
    {
        private readonly DataDbContext dbContext;
        private readonly DistrictController controller;

        public DistrictControllerTest()
        {
            dbContext = new DataDbContext(
                new DbContextOptionsBuilder<DataDbContext>()
                    .UseInMemoryDatabase(Guid.NewGuid().ToString())
                    .Options
            );
            controller = new DistrictController(dbContext);
        }

        [Fact]
        public async Task DeleteWithDependentWard()
        {
            dbContext.Districts.Add(new District
            {
                DistrictId = 1,
                DistrictName = "1"
            });
            
            dbContext.Wards.Add(new Ward
            {
                DistrictId = 1,
                WardId = 2,
                WardName = "1"
            });

            await dbContext.SaveChangesAsync();

            var deleteResult = await controller.Delete(1);
            Assert.IsAssignableFrom<BadRequestObjectResult>(deleteResult);
        }
    }
}
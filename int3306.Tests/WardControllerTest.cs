using System;
using System.Threading.Tasks;
using int3306.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace int3306.Tests;

public class WardControllerTest
{
    private readonly DataDbContext dbContext;
    private readonly WardController wardController;
    private readonly DistrictController districtController;

    public WardControllerTest()
    {
        dbContext = new DataDbContext(
            new DbContextOptionsBuilder<DataDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
        );
        wardController = new WardController(dbContext);
        districtController = new DistrictController(dbContext);
    }

    [Fact]
    public async Task AddToEmptyDb()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        const string wardName = "test";
        var createResult = await wardController.Create(new Ward { WardName = wardName, DistrictId = district.Value!.DistrictId });

        Assert.IsType<ActionResult<Ward>>(createResult);
        Assert.Equal(wardName, createResult.Value?.WardName);
        Assert.Contains(dbContext.Wards, w => w.WardName == wardName);

        var listResult = await wardController.List();
        Assert.NotNull(listResult.Value);
        Assert.Contains(listResult.Value!, w => w.WardName == wardName);
        Assert.Single(listResult.Value!);
    }

    [Fact]
    public async Task AddToNonEmptyDb()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        dbContext.Wards.Add(new Ward { WardName = "test1", DistrictId = district.Value!.DistrictId });

        const string wardName = "test";
        var createResult = await wardController.Create(new Ward { WardName = wardName, DistrictId = district.Value!.DistrictId });

        Assert.IsType<ActionResult<Ward>>(createResult);
        Assert.Equal(wardName, createResult.Value?.WardName);
        Assert.Contains(dbContext.Wards, w => w.WardName == wardName);

        var listResult = await wardController.List();
        Assert.NotNull(listResult.Value);
        Assert.Contains(listResult.Value!, w => w.WardName == wardName);
        Assert.Equal(2, listResult.Value!.Length);
    }

    [Fact]
    public async Task GetFound()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        const string wardName = "test";
        var createResult = await wardController.Create(new Ward { WardName = wardName, DistrictId = district.Value!.DistrictId });
        Assert.NotNull(createResult.Value);
        var getResult = await wardController.Get(createResult.Value!.WardId);
        Assert.NotNull(getResult.Value);
        Assert.Equal(getResult.Value!.WardName, wardName);
    }
    
    [Fact]
    public async Task GetNotFound()
    {
        var getResult = await wardController.Get(1234);
        Assert.IsAssignableFrom<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task DeleteNotFound()
    {
        var deleteResult = await wardController.Delete(1234);
        Assert.IsAssignableFrom<NotFoundResult>(deleteResult);
    }
    
    [Fact]
    public async Task DeleteFound()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        var createResult = await wardController.Create(new Ward { WardName = "1", DistrictId = district.Value!.DistrictId });
        Assert.NotNull(createResult.Value);
        var deleteResult = await wardController.Delete(createResult.Value!.WardId);
        Assert.IsAssignableFrom<OkResult>(deleteResult);
    }
    
    [Fact]
    public async Task DeleteShopExistent()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        var createResult = await wardController.Create(new Ward { WardName = "1", DistrictId = district.Value!.DistrictId });
        Assert.NotNull(createResult.Value);
        var deleteResult = await wardController.Delete(createResult.Value!.WardId);
        Assert.IsAssignableFrom<OkResult>(deleteResult);
    }

    [Fact]
    public async Task List()
    {
        var district = await districtController.Create(new District { DistrictName = "1" });
        Assert.NotNull(district.Value);
        var createResult1 = await wardController.Create(new Ward { WardName = "1", DistrictId = district.Value!.DistrictId });
        Assert.NotNull(createResult1.Value);
        var createResult2 = await wardController.Create(new Ward { WardName = "2", DistrictId = district.Value!.DistrictId });
        Assert.NotNull(createResult2.Value);
        var listResult = await wardController.List();
        Assert.NotNull(listResult.Value);
        var list = listResult.Value!;
        Assert.Equal(2, list.Length);
        Assert.Contains(list, w => w.WardId == createResult1.Value!.WardId);
        Assert.Contains(list, w => w.WardId == createResult2.Value!.WardId);
    }

    [Fact]
    public async Task ListNone()
    {
        var result = await wardController.List();
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }
}
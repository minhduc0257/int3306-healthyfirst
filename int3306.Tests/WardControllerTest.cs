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
    private readonly WardController controller;

    public WardControllerTest()
    {
        dbContext = new DataDbContext(
            new DbContextOptionsBuilder<DataDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options
        );
        controller = new WardController(dbContext);
    }

    [Fact]
    public async Task AddToEmptyDb()
    {
        const string wardName = "test";
        var createResult = await controller.Create(new Ward { WardName = wardName });

        Assert.IsType<ActionResult<Ward>>(createResult);
        Assert.Equal(wardName, createResult.Value?.WardName);
        Assert.Contains(dbContext.Wards, w => w.WardName == wardName);

        var listResult = await controller.List();
        Assert.NotNull(listResult.Value);
        Assert.Contains(listResult.Value!, w => w.WardName == wardName);
        Assert.Single(listResult.Value!);
    }

    [Fact]
    public async Task AddToNonEmptyDb()
    {
        dbContext.Wards.Add(new Ward { WardName = "test1" });
        
        const string wardName = "test";
        var createResult = await controller.Create(new Ward { WardName = wardName });

        Assert.IsType<ActionResult<Ward>>(createResult);
        Assert.Equal(wardName, createResult.Value?.WardName);
        Assert.Contains(dbContext.Wards, w => w.WardName == wardName);

        var listResult = await controller.List();
        Assert.NotNull(listResult.Value);
        Assert.Contains(listResult.Value!, w => w.WardName == wardName);
        Assert.Equal(2, listResult.Value!.Length);
    }

    [Fact]
    public async Task GetFound()
    {
        const string wardName = "test";
        var createResult = await controller.Create(new Ward { WardName = wardName });
        Assert.NotNull(createResult.Value);
        var getResult = await controller.Get(createResult.Value!.WardId);
        Assert.NotNull(getResult.Value);
        Assert.Equal(getResult.Value!.WardName, wardName);
    }
    
    [Fact]
    public async Task GetNotFound()
    {
        var getResult = await controller.Get(1234);
        Assert.IsAssignableFrom<NotFoundResult>(getResult.Result);
    }

    [Fact]
    public async Task DeleteNotFound()
    {
        var deleteResult = await controller.Delete(1234);
        Assert.IsAssignableFrom<NotFoundResult>(deleteResult);
    }
    
    [Fact]
    public async Task DeleteFound()
    {
        var createResult = await controller.Create(new Ward { WardName = "1" });
        Assert.NotNull(createResult.Value);
        var deleteResult = await controller.Delete(createResult.Value!.WardId);
        Assert.IsAssignableFrom<OkResult>(deleteResult);
    }

    [Fact]
    public async Task List()
    {
        var createResult1 = await controller.Create(new Ward { WardName = "1" });
        Assert.NotNull(createResult1.Value);
        var createResult2 = await controller.Create(new Ward { WardName = "2" });
        Assert.NotNull(createResult2.Value);
        var listResult = await controller.List();
        Assert.NotNull(listResult.Value);
        var list = listResult.Value!;
        Assert.Equal(2, list.Length);
        Assert.Contains(list, w => w.WardId == createResult1.Value!.WardId);
        Assert.Contains(list, w => w.WardId == createResult2.Value!.WardId);
    }

    [Fact]
    public async Task ListNone()
    {
        var result = await controller.List();
        Assert.NotNull(result.Value);
        Assert.Empty(result.Value!);
    }
}
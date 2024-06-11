using AutoMapper;
using Data;
using Data.Entities;
using Data.Repos;
using Domain.User;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace DataTests;

public class UserRepoTests
{
    private DataContext _dbContext;
    private IUsersRepo _repo;
    private IMapper _mapper;

    [SetUp]
    public void Setup()
    {
        var testMapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserEntity>().ReverseMap();
        });
        _mapper = testMapperConfig.CreateMapper();
        var testContextOptions = new DbContextOptionsBuilder<DataContext>()
                                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // seperate db for each test
                                    .Options;
        _dbContext = new DataContext(testContextOptions);
        _repo = new UsersRepo(_dbContext, _mapper);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }

    // TestBulkUpsertUpdatesAndInserts

    // TestCreateAsyncCreates

    // TestCreateAsyncDoesntCreateWhenIdAlreadyExists

    // TestDeleteUserDeletes

    // TestGetAllReturnsAll

    // TestGetByIdReturnsWhenExists

    // TestGetByIdReturns404WhenNotExists

    // TestUpdateUserUpdatesWhenExists

    // TestUpdateUserReturns404WhenNotExists

    // TestUpsertUpdatesWhenExists

    // TestUpsertInsertsWhenNotExists
}
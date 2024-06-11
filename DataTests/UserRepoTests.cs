using AutoMapper;
using Data;
using Data.Entities;
using Data.Repos;
using Domain.User;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace DataTests;

public class UserRepoTests
{
    private DataContext _dbContext;
    private IUsersRepo _testRepo;
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
        _testRepo = new UsersRepo(_dbContext, _mapper);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task TestBulkUpsertUpdatesAndInserts()
    {
        var userEntities = new UserEntity[]
        {
            new()
            {
                ApartmentSuite = "abc",
                City = "abc",
                CompanyBs = "abc",
                CompanyCatchPhrase = "abc",
                CompanyName = "abc",
                Email = "abc@abc.com",
                Latitude = "123",
                Longitude = "1233",
                Name = "abc",
                Phone = "123456789",
                Id = 1,
                StreetAddress = "abc",
                Username = "abc",
                Website = "abc",
                ZipCode = "abc"
            },
            new()
            {
                ApartmentSuite = "abc1",
                City = "abc1",
                CompanyBs = "abc1",
                CompanyCatchPhrase = "abc1",
                CompanyName = "abc1",
                Email = "abc@abc1.com",
                Latitude = "1231",
                Longitude = "12331",
                Name = "abc1",
                Phone = "1234567891",
                Id = 2,
                StreetAddress = "abc1",
                Username = "abc1",
                Website = "abc1",
                ZipCode = "abc1"
            },
            new()
            {
                ApartmentSuite = "abc2",
                City = "abc2",
                CompanyBs = "abc2",
                CompanyCatchPhrase = "abc2",
                CompanyName = "abc2",
                Email = "abc@abc2.com",
                Latitude = "12312",
                Longitude = "123312",
                Name = "abc12",
                Phone = "12345678912",
                Id = 3,
                StreetAddress = "abc2",
                Username = "abc2",
                Website = "abc2",
                ZipCode = "abc2"
            }
        };
        _dbContext.AddRange(userEntities);
        await _dbContext.SaveChangesAsync();

        var upsertUsers = new List<User>
        {
            new()
            {
                ApartmentSuite = "qwerty",
                City = "abc",
                CompanyBs = "abc",
                CompanyCatchPhrase = "abc",
                CompanyName = "abc",
                Email = "abc@abc.com",
                Latitude = "123",
                Longitude = "1233",
                Name = "abc",
                Phone = "123456789",
                Id = 1,
                StreetAddress = "abc",
                Username = "abc",
                Website = "abc",
                ZipCode = "abc"
            },
            new()
            {
                ApartmentSuite = "qwerty",
                City = "abc1",
                CompanyBs = "qwerty",
                CompanyCatchPhrase = "abc1",
                CompanyName = "abc1",
                Email = "abc@abc1.com",
                Latitude = "1231",
                Longitude = "12331",
                Name = "abc1",
                Phone = "1234567891",
                Id = 2,
                StreetAddress = "abc1",
                Username = "abc1",
                Website = "abc1",
                ZipCode = "abc1"
            },
            new()
            {
                ApartmentSuite = "abc2",
                City = "abc2",
                CompanyBs = "abc2",
                CompanyCatchPhrase = "abc2",
                CompanyName = "abc2",
                Email = "abc@abc2.com",
                Latitude = "12312",
                Longitude = "123312",
                Name = "abc12",
                Phone = "12345678912",
                Id = 3,
                StreetAddress = "abc2",
                Username = "abc2",
                Website = "abc2",
                ZipCode = "abc2"
            },
            new()
            {
                ApartmentSuite = "qwe",
                City = "aqwebc2",
                CompanyBs = "qwewq",
                CompanyCatchPhrase = "abc2",
                CompanyName = "abc2",
                Email = "abc@abc2.com",
                Latitude = "12312",
                Longitude = "123312",
                Name = "abc12",
                Phone = "12345678912",
                Id = 4,
                StreetAddress = "abc2",
                Username = "abc2",
                Website = "abc2",
                ZipCode = "abc2"
            }
        };
        await _testRepo.BulkUpsert(upsertUsers);

        var upsertedUserEntities = await _dbContext.Users.ToListAsync();
        var upsertedUsers = _mapper.Map<List<User>>(upsertedUserEntities);
        upsertedUsers.Should().HaveCount(upsertUsers.Count());
        upsertedUsers.Should().BeEquivalentTo(upsertUsers);
    }

    [Test]
    public async Task TestCreateAsyncCreates()
    {
        var newUser = new User
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        await _testRepo.CreateAsync(newUser);

        var createdUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var createdUser = _mapper.Map<User>(createdUserEntity);
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(newUser);
    }

    [Test]
    public async Task TestCreateAsyncThrowsApplicationExceptionWhenIdAlreadyExists()
    {
        var newUser = new UserEntity
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        _dbContext.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var userDuplicate = _mapper.Map<User>(newUser);
        var func = async () => await _testRepo.CreateAsync(userDuplicate);
        func.Should().ThrowAsync<ApplicationException>();
    }

    [Test]
    public async Task TestDeleteUserDeletes()
    {
        var userEntity = new UserEntity
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        _dbContext.Add(userEntity);
        await _dbContext.SaveChangesAsync();

        await _testRepo.DeleteUserAsync(userEntity.Id);
        var userInDbEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userEntity.Id);
        userInDbEntity.Should().BeNull();
    }

    [Test]
    public async Task TestGetAllReturnsAll()
    {
        var userEntities = new UserEntity[]
        {
            new()
            {
                ApartmentSuite = "abc",
                City = "abc",
                CompanyBs = "abc",
                CompanyCatchPhrase = "abc",
                CompanyName = "abc",
                Email = "abc@abc.com",
                Latitude = "123",
                Longitude = "1233",
                Name = "abc",
                Phone = "123456789",
                Id = 1,
                StreetAddress = "abc",
                Username = "abc",
                Website = "abc",
                ZipCode = "abc"
            },
            new()
            {
                ApartmentSuite = "abc1",
                City = "abc1",
                CompanyBs = "abc1",
                CompanyCatchPhrase = "abc1",
                CompanyName = "abc1",
                Email = "abc@abc1.com",
                Latitude = "1231",
                Longitude = "12331",
                Name = "abc1",
                Phone = "1234567891",
                Id = 2,
                StreetAddress = "abc1",
                Username = "abc1",
                Website = "abc1",
                ZipCode = "abc1"
            },
            new()
            {
                ApartmentSuite = "abc2",
                City = "abc2",
                CompanyBs = "abc2",
                CompanyCatchPhrase = "abc2",
                CompanyName = "abc2",
                Email = "abc@abc2.com",
                Latitude = "12312",
                Longitude = "123312",
                Name = "abc12",
                Phone = "12345678912",
                Id = 3,
                StreetAddress = "abc2",
                Username = "abc2",
                Website = "abc2",
                ZipCode = "abc2"
            }
        };
        _dbContext.AddRange(userEntities);
        await _dbContext.SaveChangesAsync();
        var usersInDb = _mapper.Map<ICollection<User>>(userEntities);

        var users = await _testRepo.GetAllAsync();
        Assert.That(users, Has.Count.EqualTo(usersInDb.Count));
        usersInDb.Should().BeEquivalentTo(users);
    }

    [Test]
    public async Task TestGetByIdReturnsWhenExists()
    {
        var newUser = new UserEntity
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        _dbContext.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var createdUser = await _testRepo.GetByIdAsync(newUser.Id);
        Assert.That(createdUser, Is.Not.Null);
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(_mapper.Map<User>(newUser));
    }

    [Test]
    public async Task TestGetByIdReturnsNullWhenNotExists()
    {
        var createdUser = await _testRepo.GetByIdAsync(22);
        createdUser.Should().BeNull();
    }

    [Test]
    public async Task TestUpdateUserUpdatesWhenExists()
    {
        var newUser = new UserEntity
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        _dbContext.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var updateUser = new User
        {
            ApartmentSuite = "qwerty",
            City = "qwerty",
            CompanyBs = "qwerty",
            CompanyCatchPhrase = "qwerty",
            CompanyName = "qwerty",
            Email = "qwerty@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        await _testRepo.UpdateUserAsync(updateUser);

        var updatedUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var updatedUser = _mapper.Map<User>(updatedUserEntity);
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(updatedUser);
    }

    [Test]
    public async Task TestUpdateUserThrowsApplicationExceptionWhenNotExists()
    {
        var updateUser = new User
        {
            ApartmentSuite = "qwerty",
            City = "qwerty",
            CompanyBs = "qwerty",
            CompanyCatchPhrase = "qwerty",
            CompanyName = "qwerty",
            Email = "qwerty@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 123,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };

        var func = async () => await _testRepo.UpdateUserAsync(updateUser);
        func.Should().ThrowAsync<ApplicationException>();
    }

    [Test]
    public async Task TestUpsertUpdatesWhenExists()
    {
        var newUser = new UserEntity
        {
            ApartmentSuite = "abc",
            City = "abc",
            CompanyBs = "abc",
            CompanyCatchPhrase = "abc",
            CompanyName = "abc",
            Email = "abc@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        _dbContext.Add(newUser);
        await _dbContext.SaveChangesAsync();

        var updateUser = new User
        {
            ApartmentSuite = "qwerty",
            City = "qwerty",
            CompanyBs = "qwerty",
            CompanyCatchPhrase = "qwerty",
            CompanyName = "qwerty",
            Email = "qwerty@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 1,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };
        await _testRepo.UpsertUserAsync(updateUser);

        var updatedUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var updatedUser = _mapper.Map<User>(updatedUserEntity);
        Assert.That(updatedUser, Is.Not.Null);
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(_mapper.Map<User>(updateUser));
    }

    [Test]
    public async Task TestUpsertInsertsWhenNotExists()
    {
        var updateUser = new User
        {
            ApartmentSuite = "qwerty",
            City = "qwerty",
            CompanyBs = "qwerty",
            CompanyCatchPhrase = "qwerty",
            CompanyName = "qwerty",
            Email = "qwerty@abc.com",
            Latitude = "123",
            Longitude = "1233",
            Name = "abc",
            Phone = "123456789",
            Id = 123,
            StreetAddress = "abc",
            Username = "abc",
            Website = "abc",
            ZipCode = "abc"
        };

        await _testRepo.UpsertUserAsync(updateUser);

        var userInDbEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == updateUser.Id);
        var userInDb = _mapper.Map<User>(userInDbEntity);
        userInDb.Should().NotBeNull();
        userInDb.Should().BeEquivalentTo(updateUser);
    }
}
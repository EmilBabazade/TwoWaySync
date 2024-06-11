using AutoMapper;
using Data;
using Data.Entities;
using Data.Repos;
using Domain.User;
using Microsoft.EntityFrameworkCore;

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
        await _repo.BulkUpsert(upsertUsers);

        var upsertedUserEntities = await _dbContext.Users.ToListAsync();
        var upsertedUsers = _mapper.Map<List<User>>(upsertedUserEntities);
        Assert.That(upsertedUsers, Has.Count.EqualTo(upsertUsers.Count()));
        Assert.That(upsertedUsers, Is.EqualTo(upsertUsers));
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
        await _repo.CreateAsync(newUser);

        var createdUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var createdUser = _mapper.Map<User>(createdUserEntity);
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser, Is.EqualTo(newUser));
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
        Assert.That(async () => await _repo.CreateAsync(userDuplicate), Throws.InstanceOf<ApplicationException>());
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

        await _repo.DeleteUserAsync(userEntity.Id);
        var userInDbEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == userEntity.Id);
        Assert.That(userInDbEntity, Is.Null);
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

        var users = await _repo.GetAllAsync();
        Assert.That(users, Has.Count.EqualTo(usersInDb.Count));
        foreach(var userInDb in usersInDb)
        {
            Assert.That(userInDb, Is.EqualTo(users.First(u => u.Id == userInDb.Id)));
        }
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

        var createdUser = await _repo.GetByIdAsync(newUser.Id);
        Assert.That(createdUser, Is.Not.Null);
        Assert.That(createdUser, Is.EqualTo(_mapper.Map<User>(newUser)));
    }

    [Test]
    public async Task TestGetByIdReturnsNullWhenNotExists()
    {
        var createdUser = await _repo.GetByIdAsync(22);
        Assert.That(createdUser, Is.Null);
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
        await _repo.UpdateUserAsync(updateUser);

        var updatedUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var updatedUser = _mapper.Map<User>(updatedUserEntity);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser, Is.EqualTo(updateUser));
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

        Assert.That(async () => await _repo.UpdateUserAsync(updateUser), Throws.InstanceOf<ApplicationException>());
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
        await _repo.UpsertUserAsync(updateUser);

        var updatedUserEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var updatedUser = _mapper.Map<User>(updatedUserEntity);
        Assert.That(updatedUser, Is.Not.Null);
        Assert.That(updatedUser, Is.EqualTo(_mapper.Map<User>(updateUser)));
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

        await _repo.UpsertUserAsync(updateUser);

        var userInDbEntity = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == updateUser.Id);
        var userInDb = _mapper.Map<User>(userInDbEntity);
        Assert.That(userInDb, Is.Not.Null);
        Assert.That(userInDb, Is.EqualTo(updateUser));
    }
}
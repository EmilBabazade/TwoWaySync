using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Net.Http.Headers;
using Microsoft.Extensions.DependencyInjection;
using Data;
using Data.Entities;
using Domain.User;
using FluentAssertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Services.UsersApi.ResponseModel;
using Services.UsersApi;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;

namespace IntegrationTests;

public class ActionControllerTests
{
    protected readonly CustomWebApplicationFactory<Program> _factory;
    private const string _urlRoot = "/api/Action";
    public ActionControllerTests()
    {
        _factory = new();
    }

    private async Task<HttpResponseMessage?> SendRequestAsync(string url, HttpMethod method, object? body = null)
    {
        var request = new HttpRequestMessage(method, _urlRoot + url);
        if (method != HttpMethod.Get)
        {
            var serializedBody = JsonSerializer.Serialize(body ?? new JsonObject(), new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true, 
                Converters = { new JsonStringEnumConverter() } 
            });
            var stringContent = new StringContent(serializedBody);
            stringContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Content = stringContent;
        }
        return await _factory.CreateClient().SendAsync(request);
    }

    private async Task<TResult?> SendRequestAsync<TResult>(string url, HttpMethod method, object? body = null)
    {
        var response = await SendRequestAsync(url, method, body);
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<TResult>(responseString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
    }

    private DataContext GetDataContext() => 
        _factory.Services.CreateScope().ServiceProvider.GetService<DataContext>();

    private IMapper GetMapper() => 
        _factory.Services.CreateScope().ServiceProvider.GetService<IMapper>();

    [SetUp]
    public async Task Setup()
    {
        // clear db between tests
        using var dbContext = GetDataContext();
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();
    }

    [Test]
    public async Task TestSyncLocalToRemote()
    {
        await SendRequestAsync("/SynchronizeLocalToRemote", HttpMethod.Get);
        using var dbContext = GetDataContext();
        var usersInDb = await dbContext.Users.ToListAsync();
        var mapper = GetMapper();
        var actual = mapper.Map<IEnumerable<User>>(usersInDb);
        using var httpClient = new HttpClient();
        var apiUsers = await httpClient.GetFromJsonAsync<IEnumerable<RequestUser>>(new Uri("https://jsonplaceholder.typicode.com/users"));
        var expected = mapper.Map<IEnumerable<User>>(apiUsers);
        actual.Should().BeEquivalentTo(expected);
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
        await SendRequestAsync("/Create", HttpMethod.Post, newUser);
        using var dbContext = GetDataContext();
        var createdUserEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var mapper = GetMapper();
        var createdUser = mapper.Map<User>(createdUserEntity);
        createdUser.Should().NotBeNull();
        createdUser.Should().BeEquivalentTo(newUser);
    }

    [Test]
    public async Task TestCreateAsyncReturnsBadRequestWhenIdAlreadyExists()
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
        using var dbContext = GetDataContext();
        dbContext.Add(newUser);
        await dbContext.SaveChangesAsync();

        var mapper = GetMapper();
        var userDuplicate = mapper.Map<User>(newUser);
        var func = async () => await SendRequestAsync("/Create", HttpMethod.Post, newUser);
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
        using var dbContext = GetDataContext();
        dbContext.Add(userEntity);
        await dbContext.SaveChangesAsync();

        await SendRequestAsync($"/Delete/{userEntity.Id}", HttpMethod.Delete);
        var userInDbEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userEntity.Id);
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
        using var dbContext = GetDataContext();
        dbContext.AddRange((IEnumerable<object>)userEntities);
        await dbContext.SaveChangesAsync();

        var users = await SendRequestAsync<IEnumerable<User>>("/GetAll", HttpMethod.Get);

        users.Should().HaveCount(userEntities.Count());
        var mapper = GetMapper();
        users.Should().BeEquivalentTo(mapper.Map<IEnumerable<User>>(userEntities));
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
        using var dbContext = GetDataContext();
        dbContext.Add(newUser);
        await dbContext.SaveChangesAsync();

        var createdUser = await SendRequestAsync<User>($"/Get/{newUser.Id}", HttpMethod.Get);
        createdUser.Should().NotBeNull();
        var mapper = GetMapper();
        createdUser.Should().BeEquivalentTo(mapper.Map<User>(newUser));
    }

    [Test]
    public async Task TestGetByIdReturnsNotFoundWhenNotExists()
    {
        var response = await SendRequestAsync("/Get/22", HttpMethod.Get);
        response.Should().HaveStatusCode(System.Net.HttpStatusCode.NotFound);
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
        using var dbContext = GetDataContext();
        dbContext.Add(newUser);
        await dbContext.SaveChangesAsync();

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
        await SendRequestAsync("/Update", HttpMethod.Patch, updateUser);

        var updatedUserEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var mapper = GetMapper();
        var updatedUser = mapper.Map<User>(updatedUserEntity);
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(updatedUser);
    }

    [Test]
    public async Task TestUpdateUserReturnsNotFoundWhenNotExists()
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

        var func = async () => await SendRequestAsync("/Update", HttpMethod.Patch, updateUser);
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
        using var dbContext = GetDataContext();
        dbContext.Add(newUser);
        await dbContext.SaveChangesAsync();

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
        await SendRequestAsync("/Upsert", HttpMethod.Patch, updateUser);

        var updatedUserEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == newUser.Id);
        var mapper = GetMapper();
        var updatedUser = mapper.Map<User>(updatedUserEntity);
        updatedUser.Should().NotBeNull();
        updatedUser.Should().BeEquivalentTo(updatedUser);
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

        await SendRequestAsync("/Upsert", HttpMethod.Put, updateUser);

        using var dbContext = GetDataContext();
        var userInDbEntity = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == updateUser.Id);
        var mapper = GetMapper();
        var userInDb = mapper.Map<User>(userInDbEntity);
        userInDb.Should().NotBeNull();
        userInDb.Should().BeEquivalentTo(updateUser);
    }
}
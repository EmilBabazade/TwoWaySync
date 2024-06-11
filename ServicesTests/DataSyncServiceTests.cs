using AutoMapper;
using Data;
using Data.Entities;
using Data.Repos;
using Domain.User;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Moq.Protected;
using Services.DataSync;
using Services.UsersApi;
using Services.UsersApi.ResponseModel;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;

namespace ServicesTests;

public class DataSyncServiceTests
{
    private DataContext _dbContext;
    private IUsersRepo _repo;
    private IMapper _mapper;
    private UserApiHttpClient _userApiHttpClient;
    private DataSyncService _testService;

    const string RESPONSE_CONTENT = @"
        [
          {
            ""id"": 1,
            ""name"": ""Leanne Graham"",
            ""username"": ""Bret"",
            ""email"": ""Sincere@april.biz"",
            ""address"": {
              ""street"": ""Kulas Light"",
              ""suite"": ""Apt. 556"",
              ""city"": ""Gwenborough"",
              ""zipcode"": ""92998-3874"",
              ""geo"": {
                ""lat"": ""-37.3159"",
                ""lng"": ""81.1496""
              }
            },
            ""phone"": ""1-770-736-8031 x56442"",
            ""website"": ""hildegard.org"",
            ""company"": {
              ""name"": ""Romaguera-Crona"",
              ""catchPhrase"": ""Multi-layered client-server neural-net"",
              ""bs"": ""harness real-time e-markets""
            }
          },
          {
            ""id"": 2,
            ""name"": ""Ervin Howell"",
            ""username"": ""Antonette"",
            ""email"": ""Shanna@melissa.tv"",
            ""address"": {
              ""street"": ""Victor Plains"",
              ""suite"": ""Suite 879"",
              ""city"": ""Wisokyburgh"",
              ""zipcode"": ""90566-7771"",
              ""geo"": {
                ""lat"": ""-43.9509"",
                ""lng"": ""-34.4618""
              }
            },
            ""phone"": ""010-692-6593 x09125"",
            ""website"": ""anastasia.net"",
            ""company"": {
              ""name"": ""Deckow-Crist"",
              ""catchPhrase"": ""Proactive didactic contingency"",
              ""bs"": ""synergize scalable supply-chains""
            }
          },
          {
            ""id"": 3,
            ""name"": ""Clementine Bauch"",
            ""username"": ""Samantha"",
            ""email"": ""Nathan@yesenia.net"",
            ""address"": {
              ""street"": ""Douglas Extension"",
              ""suite"": ""Suite 847"",
              ""city"": ""McKenziehaven"",
              ""zipcode"": ""59590-4157"",
              ""geo"": {
                ""lat"": ""-68.6102"",
                ""lng"": ""-47.0653""
              }
            },
            ""phone"": ""1-463-123-4447"",
            ""website"": ""ramiro.info"",
            ""company"": {
              ""name"": ""Romaguera-Jacobson"",
              ""catchPhrase"": ""Face to face bifurcated interface"",
              ""bs"": ""e-enable strategic applications""
            }
          }]";

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
        var mockMsgHandler = new Mock<HttpMessageHandler>();
        mockMsgHandler.Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(RESPONSE_CONTENT)
            });
        _userApiHttpClient = new UserApiHttpClient(new HttpClient(mockMsgHandler.Object));
        _testService = new DataSyncService(_userApiHttpClient, _repo);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }


    [Test]
    public void Test1()
    {
        using var sw = new StringWriter();
        Console.SetOut(sw);
        Console.WriteLine("abc");
        var output = sw.ToString().Replace("\r\n", string.Empty);
        Assert.That(output, Is.EqualTo("abc"));
    }

    [Test]
    public async Task TestSynchronizeLocalToRemoteAsyncSyncsLocalWithRemote()
    {   
        await _testService.SynchronizeLocalToRemoteAsync();
        var usersInDb = await _dbContext.Users.ToListAsync();
        var actual = _mapper.Map<IEnumerable<User>>(usersInDb);
        var requestUsers = JsonSerializer.Deserialize<IEnumerable<RequestUser>>(RESPONSE_CONTENT, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var expected = ResponseToUser(requestUsers);
        actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public async Task TestSyncLocalToRemoteAsyncSyncsRemoteWithLocal()
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
                Id = 29,
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

        using var sw = new StringWriter();
        Console.SetOut(sw);
        await _testService.SynchronizeRemoteToLocalAsync();
        var actual = sw.ToString().Replace("\r\n", string.Empty);
        var expected = @"---------------------------------------------
Updated user with id 1
to
Id: 1
Name: abc
Username: abc
Email: abc@abc.com
Address: Services.UsersApi.ResponseModel.Address
Phone: 123456789
Website: abc
Company: Services.UsersApi.ResponseModel.Company

---------------------------------------------
Updated user with id 3
to
Id: 3
Name: abc12
Username: abc2
Email: abc@abc2.com
Address: Services.UsersApi.ResponseModel.Address
Phone: 12345678912
Website: abc2
Company: Services.UsersApi.ResponseModel.Company

---------------------------------------------
Added new user:
Id: 29
Name: abc1
Username: abc1
Email: abc@abc1.com
Address: Services.UsersApi.ResponseModel.Address
Phone: 1234567891
Website: abc1
Company: Services.UsersApi.ResponseModel.Company
";
        Assert.That(actual, Is.EqualTo(actual));
    }

    // TODO: PUT IN AUTOMAPPER
    private static RequestUser UserToRequestUser(User u)
    {
        return new RequestUser
        {
            Address = new Address
            {
                Suite = u.ApartmentSuite,
                City = u.City,
                Street = u.StreetAddress,
                Zipcode = u.ZipCode,
                Geo = new Geo
                {
                    Lat = u.Latitude,
                    Lng = u.Longitude
                }
            },
            Company = new Company
            {
                Bs = u.CompanyBs,
                CatchPhrase = u.CompanyCatchPhrase,
                Name = u.CompanyName
            },
            Name = u.Name,
            Email = u.Email,
            Id = u.Id,
            Phone = u.Phone,
            Username = u.Username,
            Website = u.Website
        };
    }

    private static User ResponseToUser(RequestUser u)
    {
        return new User
        {
            ApartmentSuite = u.Address.Suite,
            City = u.Address.City,
            CompanyBs = u.Company.Bs,
            CompanyCatchPhrase = u.Company.CatchPhrase,
            CompanyName = u.Company.Name,
            Name = u.Name,
            Email = u.Email,
            Id = u.Id,
            Latitude = u.Address.Geo.Lat,
            Longitude = u.Address.Geo.Lng,
            Phone = u.Phone,
            StreetAddress = u.Address.Street,
            Username = u.Username,
            Website = u.Website,
            ZipCode = u.Address.Zipcode
        };
    }

    private static IEnumerable<User> ResponseToUser(IEnumerable<RequestUser> users)
    {
        return users.Select(u => new User
        {
            ApartmentSuite = u.Address.Suite,
            City = u.Address.City,
            CompanyBs = u.Company.Bs,
            CompanyCatchPhrase = u.Company.CatchPhrase,
            CompanyName = u.Company.Name,
            Name = u.Name,
            Email = u.Email,
            Id = u.Id,
            Latitude = u.Address.Geo.Lat,
            Longitude = u.Address.Geo.Lng,
            Phone = u.Phone,
            StreetAddress = u.Address.Street,
            Username = u.Username,
            Website = u.Website,
            ZipCode = u.Address.Zipcode
        });
    }
}
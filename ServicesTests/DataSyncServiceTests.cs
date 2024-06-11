using AutoMapper;
using Data;
using Data.Entities;
using Data.MappingProfiles;
using Data.Repos;
using Domain.User;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Services.DataSync;
using Services.MappingProfiles;
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
    private IDataSyncService _testService;

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
            cfg.AddProfile<UserEntityMappingProfile>();
            cfg.AddProfile<RequestUserMappingProfile>();
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
        var inMemorySettings = new Dictionary<string, string> {
                { "UserApiURL", "https://emilbabazade.com" } 
        };
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
        _userApiHttpClient = new UserApiHttpClient(new HttpClient(mockMsgHandler.Object), configuration);
        _testService = new DataSyncService(_userApiHttpClient, _repo, _mapper);
    }

    [TearDown]
    public void Cleanup()
    {
        _dbContext.Dispose();
    }

    [Test]
    public async Task TestSynchronizeLocalToRemoteAsyncSyncsLocalWithRemote()
    {   
        await _testService.SynchronizeLocalToRemoteAsync();
        var usersInDb = await _dbContext.Users.ToListAsync();
        var actual = _mapper.Map<IEnumerable<User>>(usersInDb);
        var requestUsers = JsonSerializer.Deserialize<IEnumerable<RequestUser>>(RESPONSE_CONTENT, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        var expected = _mapper.Map<IEnumerable<User>>(requestUsers);
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
        actual.Should().BeEquivalentTo(actual);
    }
}
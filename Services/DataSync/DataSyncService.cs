using Data.Repos;
using Domain.User;
using Services.UsersApi;
using Services.UsersApi.ResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Services.DataSync;
public class DataSyncService
{
    private readonly UserApiHttpClient _userApiHttpClient;
    private readonly IUsersRepo _usersRepo;

    public DataSyncService(UserApiHttpClient userApiHttpClient, IUsersRepo usersRepo)
    {
        _userApiHttpClient = userApiHttpClient;
        _usersRepo = usersRepo;
    }

    public async Task SynchronizeLocalToRemoteAsync(CancellationToken cancellationToken = default)
    {
        var response = await _userApiHttpClient.GetUsersAsync(cancellationToken);
        var remoteUsers = ResponseToUser(response);
        await _usersRepo.BulkUpsert(remoteUsers, cancellationToken);
    }

    public async Task SynchronizeRemoteToLocalAsync(CancellationToken cancellationToken = default)
    {
        var localUsers = await _usersRepo.GetAllAsync(cancellationToken);
        var apiUsers = await _userApiHttpClient.GetUsersAsync(cancellationToken);
        foreach(var localUser in localUsers)
        {
            var apiUser = apiUsers.FirstOrDefault(u => u.Id == localUser.Id);
            if (apiUser == null)
            {
                _userApiHttpClient.AddUserAsync(UserToRequestUser(localUser));
            }
            else
            {
                var remoteUser = ResponseToUser(apiUser);
                if (remoteUser != localUser)
                {
                    _userApiHttpClient.UpdateUserAsync(UserToRequestUser(localUser), cancellationToken);
                }
            }
        }
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

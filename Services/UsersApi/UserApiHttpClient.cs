using Domain.User;
using Services.UsersApi.ResponseModel;
using System.Net.Http.Json;

// TODO: unit tests

namespace Services.UsersApi;
public class UserApiHttpClient
{
    private readonly HttpClient _httpClient;

    public UserApiHttpClient(HttpClient httpClient)
    {
        _httpClient = httpClient;

        // TODO: move to appsettings.json
        _httpClient.BaseAddress = new Uri("https://jsonplaceholder.typicode.com/users");
    }

    public async Task<ICollection<User>?> GetUsers(CancellationToken cancellationToken = default)
    {
        var responseUsers = await _httpClient.GetFromJsonAsync<IEnumerable<ResponseUser>?>(string.Empty, cancellationToken);
        // TODO: null checks
        return responseUsers.Select(u => new User
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
        }).ToList();
    }
}

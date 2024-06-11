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

    public async Task<ICollection<User>?> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        var responseUsers = await _httpClient.GetFromJsonAsync<IEnumerable<ResponseUser>?>(string.Empty, cancellationToken);
        // TODO: null checks
        return responseUsers.Select(ResponseToUser).ToList();
    }

    public async Task<User?> GetUserAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            // What is the point of having a _httpClient.BaseAddress if the url isn't gonna be automatically appended to it ???
            var res = await _httpClient.GetFromJsonAsync<ResponseUser?>($"{_httpClient.BaseAddress}/{id}", cancellationToken);
            return ResponseToUser(res);
        }
        catch(HttpRequestException ex) when (ex.Message.Contains("404"))
        {
            return null;
        }
    }

    public async Task<User> AddUserAsync(User user, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Added new user:\n{user}");
        return user;
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var remoteUser = await GetUserAsync(user.Id, cancellationToken);
        Console.WriteLine($"Updated user\n{remoteUser}\nto\n{user}");
        return user;
    }

    // TODO: PUT IN AUTOMAPPER
    private static User ResponseToUser(ResponseUser u)
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
}

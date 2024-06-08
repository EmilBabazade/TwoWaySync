using Services.UsersApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;

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

    public async Task<IEnumerable<User>?> GetUsers(CancellationToken cancellationToken = default) =>
        await _httpClient.GetFromJsonAsync<IEnumerable<User>?>(string.Empty, cancellationToken);
}

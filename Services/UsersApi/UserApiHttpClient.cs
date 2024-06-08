using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
}

﻿using Domain.User;
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

    public async Task<IEnumerable<RequestUser>?> GetUsersAsync(CancellationToken cancellationToken = default) =>
        await _httpClient.GetFromJsonAsync<IEnumerable<RequestUser>?>(string.Empty, cancellationToken);

    public async Task<RequestUser?> GetUserAsync(int id, CancellationToken cancellationToken = default) => 
        await _httpClient.GetFromJsonAsync<RequestUser?>($"{_httpClient.BaseAddress}/{id}", cancellationToken); // this should be ok with just passing the subpath but it expects full url

    public async Task AddUserAsync(RequestUser user, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("---------------------------------------------\n");
        Console.WriteLine($"Added new user:\n{user}\n");
    }

    public async Task UpdateUserAsync(RequestUser user, CancellationToken cancellationToken = default)
    {
        Console.WriteLine("---------------------------------------------\n");
        Console.WriteLine($"Updated user with id {user.Id}\nto\n{user}\n");
    }
}

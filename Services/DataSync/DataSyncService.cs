using Data.Repos;
using Services.UsersApi;
using System;
using System.Collections.Generic;
using System.Linq;
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
        var remoteUsers = await _userApiHttpClient.GetUsersAsync(cancellationToken);
        await _usersRepo.BulkUpsert(remoteUsers, cancellationToken);
    }

    public async Task SynchronizeRemoteToLocalAsync(CancellationToken cancellationToken = default)
    {
        var localUsers = await _usersRepo.GetAllAsync(cancellationToken);
        foreach(var localUser in localUsers)
        {
            var remoteUser = await _userApiHttpClient.GetUserAsync(localUser.Id, cancellationToken);
            if(remoteUser == null)
            {
                _userApiHttpClient.AddUserAsync(localUser);
            } 
            else if(remoteUser != localUser)
            {
                _userApiHttpClient.UpdateUserAsync(localUser, cancellationToken);
            }
        }
    }
}

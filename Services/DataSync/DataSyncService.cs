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
    private readonly UsersRepo _usersRepo;

    public DataSyncService(UserApiHttpClient userApiHttpClient, UsersRepo usersRepo)
    {
        _userApiHttpClient = userApiHttpClient;
        _usersRepo = usersRepo;
    }

    public async Task RemoteToLocalAsync(CancellationToken cancellationToken = default)
    {
        var remoteUsers = await _userApiHttpClient.GetUsers(cancellationToken);
        await _usersRepo.BulkUpsert(remoteUsers, cancellationToken);
    }

    public async Task LocalToRemoteAsync()
    {
        throw new NotImplementedException();
    }
}

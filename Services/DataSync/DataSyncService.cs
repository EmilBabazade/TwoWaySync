using AutoMapper;
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
public class DataSyncService(UserApiHttpClient userApiHttpClient, IUsersRepo usersRepo, IMapper mapper) : IDataSyncService
{
    private readonly UserApiHttpClient _userApiHttpClient = userApiHttpClient;
    private readonly IUsersRepo _usersRepo = usersRepo;
    private readonly IMapper _mapper = mapper;

    public async Task SynchronizeLocalToRemoteAsync(CancellationToken cancellationToken = default)
    {
        var response = await _userApiHttpClient.GetUsersAsync(cancellationToken);
        var remoteUsers = _mapper.Map<IEnumerable<User>>(response);
        await _usersRepo.BulkUpsertAsync(remoteUsers, cancellationToken);
    }

    public async Task SynchronizeRemoteToLocalAsync(CancellationToken cancellationToken = default)
    {
        var localUsers = await _usersRepo.GetAllAsync(cancellationToken);
        var apiUsers = await _userApiHttpClient.GetUsersAsync(cancellationToken);
        foreach (var localUser in localUsers)
        {
            var apiUser = apiUsers.FirstOrDefault(u => u.Id == localUser.Id);
            if (apiUser == null)
            {
                _userApiHttpClient.AddUserAsync(_mapper.Map<RequestUser>(localUser));
            }
            else
            {
                var remoteUser = _mapper.Map<User>(apiUser);
                if (remoteUser != localUser)
                {
                    _userApiHttpClient.UpdateUserAsync(_mapper.Map<RequestUser>(localUser), cancellationToken);
                }
            }
        }
    }
}

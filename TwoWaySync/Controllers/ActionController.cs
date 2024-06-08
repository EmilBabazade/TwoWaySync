using Microsoft.AspNetCore.Mvc;
using Services.UsersApi;
using Services.UsersApi.Model;

namespace TwoWaySync.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly UserApiHttpClient _userApiHttpClient;

    public ActionController(UserApiHttpClient userApiHttpClient)
    {
        _userApiHttpClient = userApiHttpClient;
    }

    [HttpGet]
    [Route("GetAllUsersFromApi")]
    public async Task<IEnumerable<User>?> GetAllUsersFromApi(CancellationToken cancellationToken = default)
    {
        return await _userApiHttpClient.GetUsers(cancellationToken);
    }
}

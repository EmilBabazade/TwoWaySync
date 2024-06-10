using Data.Repos;
using Domain.User;
using Microsoft.AspNetCore.Mvc;
using Services.UsersApi;

namespace TwoWaySync.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly UsersRepo _usersRepo;
    private readonly UserApiHttpClient _userApiHttpClient;

    public ActionController(UsersRepo usersRepo, UserApiHttpClient userApiHttpClient)
    {
        _usersRepo = usersRepo;
        _userApiHttpClient = userApiHttpClient;
    }

    // for debugging
    [HttpGet]
    [Route("GetAllFromRemote")]
    public async Task<ActionResult<ICollection<User>>> GetAllFromRemote()
    {
        return Ok(await _userApiHttpClient.GetUsers());
    }

    [HttpGet]
    [Route("Get/{id}")]
    public async Task<ActionResult<User>> GetByIdAsnyc([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _usersRepo.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound();
        return result;
    }

    [HttpGet]
    [Route("GetAll")]
    public async Task<ICollection<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
        await _usersRepo.GetAllAsync(cancellationToken);
}

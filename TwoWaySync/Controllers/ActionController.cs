using Data.Repos;
using Domain.User;
using Microsoft.AspNetCore.Mvc;
using Services.DataSync;
using Services.UsersApi;

namespace TwoWaySync.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly IUsersRepo _usersRepo;
    private readonly UserApiHttpClient _userApiHttpClient;
    private readonly DataSyncService _dataSyncService;

    public ActionController(IUsersRepo usersRepo, UserApiHttpClient userApiHttpClient, DataSyncService dataSyncService)
    {
        _usersRepo = usersRepo;
        _userApiHttpClient = userApiHttpClient;
        _dataSyncService = dataSyncService;
    }

    // for debugging
    [HttpGet]
    [Route("GetAllFromRemote")]
    public async Task<ActionResult<ICollection<User>>> GetAllFromRemote(CancellationToken cancellationToken = default)
    {
        return Ok(await _userApiHttpClient.GetUsersAsync(cancellationToken));
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

    [HttpPost]
    [Route("Create")]
    public async Task<ActionResult<User>> CreateUser([FromBody] User user, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _usersRepo.CreateAsync(user, cancellationToken);
        }
        catch(ApplicationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete]
    [Route("Delete/{id}")]
    public async Task<ActionResult> Delete([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        await _usersRepo.DeleteUserAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpPatch]
    [Route("Update")]
    public async Task<ActionResult<User>> Update([FromBody] User user, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _usersRepo.UpdateUserAsync(user, cancellationToken);
        }
        catch(ApplicationException)
        {
            return NotFound();
        }
    }

    [HttpPut]
    [Route("Upsert")]
    public async Task<User> Upsert([FromBody] User user, CancellationToken cancellationToken = default) =>
        await _usersRepo.UpsertUserAsync(user, cancellationToken);

    [HttpGet]
    [Route("SynchronizeLocalToRemote")]
    public async Task<ActionResult> SynchronizeLocalToRemote(CancellationToken cancellationToken = default)
    {
        await _dataSyncService.SynchronizeLocalToRemoteAsync(cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Route("SynchronizeRemoteToLocal")]
    public async Task<ActionResult> SynchronizeRemoteToLocal(CancellationToken cancellationToken = default)
    {
        await _dataSyncService.SynchronizeRemoteToLocalAsync(cancellationToken);
        return Ok();
    }
}

using Data.Entities;
using Data.Repos;
using Microsoft.AspNetCore.Mvc;
using Services.UsersApi;
using Services.UsersApi.Model;

namespace TwoWaySync.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ActionController : ControllerBase
{
    private readonly UsersRepo _usersRepo;

    public ActionController(UsersRepo usersRepo)
    {
        _usersRepo = usersRepo;
    }

    [HttpGet]
    [Route("Get/{id}")]
    public async Task<ActionResult<UserEntity>> GetByIdAsnyc([FromRoute] int id, CancellationToken cancellationToken = default)
    {
        var result = await _usersRepo.GetByIdAsync(id, cancellationToken);
        if (result == null)
            return NotFound();
        return result;
    }
}

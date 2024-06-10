using AutoMapper;
using Domain.User;
using Microsoft.EntityFrameworkCore;

// TODO: interface
// TODO: unit tests

namespace Data.Repos;
public class UsersRepo
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public UsersRepo(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return _mapper.Map<User>(user);
    }

    public async Task<ICollection<User>> GetAllAsync(CancellationToken cancellation = default)
    {
        var users = await _dataContext.Users.ToListAsync(cancellation);
        return _mapper.Map<ICollection<User>>(users);
    }
}

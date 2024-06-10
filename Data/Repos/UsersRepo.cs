using AutoMapper;
using Data.Entities;
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
    public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
    {
        if (await _dataContext.Users.AnyAsync(u => u.Id == user.Id, cancellationToken))
            throw new ApplicationException("User already exists");

        var userEntity = _mapper.Map<UserEntity>(user);
        _dataContext.Add(userEntity);
        await _dataContext.SaveChangesAsync(cancellationToken);

        return _mapper.Map<User>(userEntity);
    }

    public async Task DeleteUserAsync(int id, CancellationToken cancellation = default)
    {
        var users = await _dataContext.Users.Where(u => u.Id == id).ToListAsync(cancellation);
        _dataContext.RemoveRange(users);
        await _dataContext.SaveChangesAsync();
    }
}

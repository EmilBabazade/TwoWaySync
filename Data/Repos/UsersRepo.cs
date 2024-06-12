using AutoMapper;
using Data.Entities;
using Domain.User;
using Microsoft.EntityFrameworkCore;

namespace Data.Repos;
public class UsersRepo(DataContext dataContext, IMapper mapper) : IUsersRepo
{
    private readonly DataContext _dataContext = dataContext;
    private readonly IMapper _mapper = mapper;

    public async Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
        return _mapper.Map<User>(user);
    }

    public async Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellation = default)
    {
        var users = await _dataContext.Users.OrderBy(u => u.Id).ToListAsync(cancellation);
        return _mapper.Map<IEnumerable<User>>(users);
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
        await _dataContext.SaveChangesAsync(cancellation);
    }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken) ?? throw new ApplicationException();
        _mapper.Map<User, UserEntity>(user, userEntity);
        await _dataContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<User>(userEntity);
    }

    public async Task<User> UpsertUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (userEntity == null)
            return await CreateAsync(user, cancellationToken);
        _mapper.Map<User, UserEntity>(user, userEntity);
        await _dataContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<User>(userEntity);
    }
    public async Task BulkUpsertAsync(IEnumerable<User> users, CancellationToken cancellationToken = default)
    {
        // process in chunks of n so the entity tracking doesn't get too big
        var chunks = users.Chunk(1000);
        foreach (var chunk in chunks)
        {
            var userIds = chunk.Select(u => u.Id);
            var existingUserEntities = await _dataContext.Users.Where(ue => userIds.Contains(ue.Id)).ToListAsync(cancellationToken);
            // insert
            var newUsers = chunk.Where(u => !existingUserEntities.Exists(ue => ue.Id == u.Id)).ToList();
            _dataContext.AddRange(_mapper.Map<List<UserEntity>>(newUsers));
            // update
            foreach (var ue in existingUserEntities)
            {
                var userToUpdate = chunk.FirstOrDefault(u => u.Id == ue.Id);
                if(userToUpdate == null) continue;
                _mapper.Map<User, UserEntity>(userToUpdate, ue);
            }
            await _dataContext.SaveChangesAsync(cancellationToken);
            _dataContext.ChangeTracker.Clear();
        }
    }
}

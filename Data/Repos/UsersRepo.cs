using AutoMapper;
using Data.Entities;
using Domain.User;
using Microsoft.EntityFrameworkCore;

// TODO: interface
// TODO: unit tests

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

    public async Task<ICollection<User>> GetAllAsync(CancellationToken cancellation = default)
    {
        var users = await _dataContext.Users.OrderBy(u => u.Id).ToListAsync(cancellation);
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

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken) ?? throw new ApplicationException();
        if (userEntity == null)
            throw new ApplicationException("User does not exist");
        UpdateUserEntity(user, userEntity);
        await _dataContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<User>(userEntity);
    }

    public async Task<User> UpsertUserAsync(User user, CancellationToken cancellationToken = default)
    {
        var userEntity = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);
        if (userEntity == null)
            return await CreateAsync(user, cancellationToken);
        UpdateUserEntity(user, userEntity);
        await _dataContext.SaveChangesAsync(cancellationToken);
        return _mapper.Map<User>(userEntity);
    }
    public async Task BulkUpsert(IEnumerable<User> users, CancellationToken cancellationToken = default)
    {
        var existingUsers = await _dataContext.Users.Where(ue => users.Select(u => u.Id).Contains(ue.Id)).ToListAsync(cancellationToken);
        // insert
        var newUsers = users.Where(u => !existingUsers.Exists(ue => ue.Id == u.Id)).ToList();
        _dataContext.AddRange(_mapper.Map<List<UserEntity>>(newUsers));
        // update
        foreach (var ue in existingUsers)
        {
            UpdateUserEntity(users.FirstOrDefault(u => u.Id == ue.Id), ue);
        }
        await _dataContext.SaveChangesAsync(cancellationToken);
    }
    private static void UpdateUserEntity(User user, UserEntity userEntity)
    {
        if (user == null || userEntity == null) return;
        userEntity.Name = user.Name;
        userEntity.Username = user.Username;
        userEntity.Email = user.Email;
        userEntity.StreetAddress = user.StreetAddress;
        userEntity.ApartmentSuite = user.ApartmentSuite;
        userEntity.City = user.City;
        userEntity.ZipCode = user.ZipCode;
        userEntity.Latitude = user.Latitude;
        userEntity.Longitude = user.Longitude;
        userEntity.Phone = user.Phone;
        userEntity.Website = user.Website;
        userEntity.CompanyName = user.CompanyName;
        userEntity.CompanyCatchPhrase = user.CompanyCatchPhrase;
        userEntity.CompanyBs = user.CompanyBs;
    }


}

using Domain.User;

namespace Data.Repos;
public interface IUsersRepo
{
    Task BulkUpsertAsync(IEnumerable<User> users, CancellationToken cancellationToken = default);
    Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
    Task DeleteUserAsync(int id, CancellationToken cancellation = default);
    Task<IEnumerable<User>> GetAllAsync(CancellationToken cancellation = default);
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    Task<User> UpsertUserAsync(User user, CancellationToken cancellationToken = default);
}
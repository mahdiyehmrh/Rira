using Rira.Domain.Users;
using Rira.Domain.Users.ValueObjects;

namespace Rira.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<User?> GetByIdReadOnlyAsync(int id, CancellationToken cancellationToken);
    Task<bool> ExistsByNationalCodeAsync(NationalCode code, int? excludeUserId, CancellationToken cancellationToken);
    Task<(IReadOnlyList<User> Items, int TotalCount)> ListAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task AddAsync(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    void Remove(User user);
}

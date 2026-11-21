using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Rira.Application.Common.Exceptions;
using Rira.Application.Interfaces;
using Rira.Domain.Users;
using Rira.Domain.Users.ValueObjects;

namespace Rira.Infrastructure.Persistence;

public sealed class UserRepository(AppDbContext db) : IUserRepository
{
    public Task<User?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return db.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<User?> GetByIdReadOnlyAsync(int id, CancellationToken cancellationToken)
    {
        return db.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<bool> ExistsByNationalCodeAsync(NationalCode code, int? excludeUserId, CancellationToken cancellationToken)
    {
        return db.Users.AsNoTracking().AnyAsync(user => user.NationalCode == code && (!excludeUserId.HasValue || user.Id != excludeUserId.Value), cancellationToken);
    }

    public async Task<(IReadOnlyList<User> Items, int TotalCount)> ListAsync(int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = db.Users.AsNoTracking().OrderBy(x => x.Id);
        var count = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, count);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken)
    {
        await db.Users.AddAsync(user, cancellationToken);
    }

    public void Remove(User user)
    {
        db.Users.Remove(user);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await db.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (exception.InnerException is SqlException { Number: 2601 or 2627 })
        {
            throw new ConflictException("A user with this NationalCode already exists.");
        }
    }
}

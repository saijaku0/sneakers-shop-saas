using Microsoft.EntityFrameworkCore;

using SneakersShop.Application.Abstractions.Repositories;
using SneakersShop.Application.Common.Exceptions;

namespace SneakersShop.Infrastructure.Persistence.Repositories;

internal sealed class UnitOfWork(AppDbContext appDbContext) : IUnitOfWork
{
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await appDbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new ConcurrencyConflictException(
                "The record was modified by another operation. Retry the command.", ex);
        }
    }
}
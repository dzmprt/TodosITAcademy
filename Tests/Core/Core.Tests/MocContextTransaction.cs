using Core.Application.Abstractions.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Core.Tests;

public class MocContextTransaction : IContextTransaction
{
    private readonly IDbContextTransaction _contextTransaction;

    public MocContextTransaction(IDbContextTransaction contextTransaction)
    {
        _contextTransaction = contextTransaction;
    }
    
    public async Task CommitAsync(CancellationToken cancellationToken)
    {
        await _contextTransaction.RollbackAsync(cancellationToken);
    }

    public async Task RollbackAsync(CancellationToken cancellationToken)
    {
        await _contextTransaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _contextTransaction.Dispose();
    }

    public ValueTask DisposeAsync()
    {
        return _contextTransaction.DisposeAsync();
    }
}
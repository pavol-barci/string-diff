using StringDiff.Domain.Models;
using StringDiff.Domain.Repositories;

namespace StringDiff.Infrastructure.Repositories;

/// <summary>
/// Just a template, this should be created with real database implementation, for example Cosmos/Mongo DB
/// </summary>
public class DiffRepository : IDiffRepository
{
    /// <inheritdoc/>
    public Task<DiffModel?> GetById(Guid id)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<DiffModel> Update(DiffModel model)
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public Task<DiffModel> Create(DiffModel model)
    {
        throw new NotSupportedException();
    }
}
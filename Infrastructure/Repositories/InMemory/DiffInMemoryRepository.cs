using StringDiff.Domain;
using StringDiff.Infrastructure.Exceptions;

namespace StringDiff.Infrastructure.Repositories.InMemory;

/// <inheritdoc/>
public class DiffInMemoryRepository : IDiffRepository
{
    private Dictionary<int, DiffModel> Data { get; set; } = new();
    
    /// <inheritdoc/>
    public Task<DiffModel?> GetById(int id)
    {
        return Data.TryGetValue(id, out var model) is false
            ? Task.FromResult<DiffModel?>(null)
            : Task.FromResult(model);
    }

    /// <inheritdoc/>
    public Task<DiffModel> Update(DiffModel model)
    {
        if (Data.ContainsKey(model.Id) is false)
        {
            throw new NotFoundException($"Model with id {model.Id} does not exists.");
        }
        
        Data[model.Id] = model;

        return Task.FromResult(model);
    }

    /// <inheritdoc/>
    public Task<DiffModel> Create(DiffModel model)
    {
        if (Data.TryAdd(model.Id, model) is false)
        {
            throw new ConflictException($"Model with id {model.Id} already exists");
        }

        return Task.FromResult(model);
    }
}